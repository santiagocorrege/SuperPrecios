using SuperPrecios.Application.DTO.MiniPSS;
using SuperPrecios.Application.IServices.Matcher;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CategoriaCore = SuperPrecios.Domain.Entities.Categoria;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;
using PrecioHistoricoCore = SuperPrecios.Domain.Entities.PrecioHistorico;
using MarcaCore = SuperPrecios.Domain.Entities.Marca;
using SupermercadoCore = SuperPrecios.Domain.Entities.Supermercado;

namespace SuperPrecios.Application.Services
{
    public class MatchingProcessService : IMatchingProcessService
    {
        private readonly IMatchingRepository _matchingRepository;

        public MatchingProcessService(IMatchingRepository matchingRepository)
        {
            _matchingRepository = matchingRepository;
        }

        public async Task ProcessMatchingResultsAsync(MiniPssResultadoDto matchingResults)
        {
            if (matchingResults?.Resultados == null)
                throw new ArgumentException("Los resultados del matching no pueden ser nulos");

            Console.WriteLine($"[INFO] Iniciando procesamiento de {matchingResults.Resultados.Count} supermercados con {matchingResults.TotalProductos} productos");
            var tiempoInicio = DateTime.Now;

            try
            {
                // ✅ PASO 1: Validar supermercados y categorías usando el repositorio optimizado
                var supermercadosIds = matchingResults.Resultados.Select(r => r.Supermercado.Id).Distinct();
                var categoriasIds = matchingResults.Resultados.Select(r => r.Ruta.Id).Distinct();
                await _matchingRepository.ValidateSupermercadosAndCategoriasAsync(supermercadosIds, categoriasIds);

                // ✅ PASO 2: NUEVA VALIDACIÓN - Verificar integridad de productos matched
                await ValidateProductosMatchedAsync(matchingResults);

                // ✅ PASO 3: Obtener entidades existentes usando repositorio optimizado (evita N+1)
                var entidadesExistentes = await GetEntidadesExistentesAsync(matchingResults);

                // ✅ PASO 4: Procesar entidades en memoria
                var entidadesProcesadas = GetEntidadesProcesadas(matchingResults, entidadesExistentes);

                // ✅ PASO 5: Ejecutar operación transaccional usando repositorio optimizado
                await _matchingRepository.ProcessMatchingTransactionAsync(
                    entidadesProcesadas.MarcasNuevas,
                    entidadesProcesadas.ProductosNuevos,
                    entidadesProcesadas.PreciosHistoricos);

                var tiempoTotal = DateTime.Now - tiempoInicio;
                Console.WriteLine($"[SUCCESS] Procesamiento completo exitoso en {tiempoTotal.TotalSeconds:F2} segundos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en procesamiento: {ex.Message}");
                throw new Exception($"Error procesando resultados de matching: {ex.Message}", ex);
            }
        }

        // ✅ NUEVO: Validación de integridad para productos matched
        private async Task ValidateProductosMatchedAsync(MiniPssResultadoDto matchingResults)
        {
            Console.WriteLine("[INFO] Validando integridad de productos matched...");

            // Obtener todos los IDs de productos que Mini PSS dice que están matched
            var productosMatchedIds = matchingResults.Resultados
                .SelectMany(r => r.ProductosProcesados
                    .Where(p => p.Matched && p.ProductoId.HasValue)
                    .Select(p => p.ProductoId.Value))
                .Distinct()
                .ToList();

            if (!productosMatchedIds.Any())
            {
                Console.WriteLine("[INFO] No hay productos matched para validar");
                return;
            }

            Console.WriteLine($"[INFO] Validando {productosMatchedIds.Count} productos matched...");

            // Verificar que realmente existan en la base de datos
            var productosExistentes = await _matchingRepository.GetProductosExistentesByIdsAsync(productosMatchedIds);

            var productosMatchedInvalidos = productosMatchedIds
                .Except(productosExistentes)
                .ToList();

            if (productosMatchedInvalidos.Any())
            {
                var idsInvalidos = string.Join(", ", productosMatchedInvalidos);
                Console.WriteLine($"[ERROR] Productos matched no existen en BD: {idsInvalidos}");
                throw new ArgumentException($"Mini PSS envió productos matched que no existen en la base de datos: {idsInvalidos}");
            }

            Console.WriteLine($"[SUCCESS] Todos los {productosMatchedIds.Count} productos matched son válidos");
        }

        private async Task<EntidadesExistentesDto> GetEntidadesExistentesAsync(MiniPssResultadoDto matchingResults)
        {
            Console.WriteLine("[INFO] Obteniendo entidades existentes...");

            // Obtener IDs únicos de marcas y productos
            var marcasIds = matchingResults.Resultados
                .SelectMany(r => r.MarcasProcesadas.Select(m => m.MarcaId))
                .Distinct();

            var productosIds = matchingResults.Resultados
                .SelectMany(r => r.ProductosProcesados
                    .Where(p => p.ProductoId.HasValue)
                    .Select(p => p.ProductoId.Value))
                .Distinct();

            // ✅ USAR REPOSITORIO OPTIMIZADO para consultas batch
            var marcasExistentes = await _matchingRepository.GetMarcasExistentesByIdsAsync(marcasIds);
            var productosExistentes = await _matchingRepository.GetProductosExistentesByIdsAsync(productosIds);

            var entidadesExistentes = new EntidadesExistentesDto
            {
                MarcasExistentes = marcasExistentes,
                ProductosExistentes = productosExistentes
            };

            Console.WriteLine($"[INFO] Entidades obtenidas - Marcas: {entidadesExistentes.MarcasExistentes.Count}, Productos: {entidadesExistentes.ProductosExistentes.Count}");

            return entidadesExistentes;
        }

        private EntidadesProcesadasDto GetEntidadesProcesadas(
            MiniPssResultadoDto matchingResults,
            EntidadesExistentesDto entidadesExistentes)
        {
            Console.WriteLine("[INFO] Procesando entidades en memoria...");

            var entidadesProcesadas = new EntidadesProcesadasDto();

            foreach (var resultado in matchingResults.Resultados)
            {
                // Procesar marcas nuevas
                var marcasNuevas = GetMarcasNuevasBySupermercado(
                    resultado.MarcasProcesadas,
                    entidadesExistentes.MarcasExistentes);
                entidadesProcesadas.MarcasNuevas.AddRange(marcasNuevas);

                // Procesar productos nuevos (NO matched)
                var productosNuevos = GetProductosNuevosBySupermercado(
                    resultado.ProductosProcesados,
                    entidadesExistentes.ProductosExistentes,
                    resultado.Ruta.Id);
                entidadesProcesadas.ProductosNuevos.AddRange(productosNuevos);

                // ✅ NUEVO: Procesar precios de productos MATCHED
                var preciosProductosMatched = GetPreciosHistoricosProductosMatched(
                    resultado.ProductosProcesados,
                    resultado.Supermercado.Id);
                entidadesProcesadas.PreciosHistoricos.AddRange(preciosProductosMatched);

                // Procesar precios de productos NUEVOS
                var preciosProductosNuevos = GetPreciosHistoricosProductosNuevos(
                    resultado.ProductosProcesados,
                    resultado.Supermercado.Id);
                entidadesProcesadas.PreciosHistoricos.AddRange(preciosProductosNuevos);
            }

            Console.WriteLine($"[INFO] Procesamiento completado - Marcas: {entidadesProcesadas.MarcasNuevas.Count}, Productos nuevos: {entidadesProcesadas.ProductosNuevos.Count}, Precios: {entidadesProcesadas.PreciosHistoricos.Count}");

            return entidadesProcesadas;
        }

        #region Métodos Get Para Procesamiento en Memoria

        private List<MarcaCore> GetMarcasNuevasBySupermercado(
            List<MarcaProcesadaDto> marcasProcesadas,
            HashSet<int> marcasExistentes)
        {
            return marcasProcesadas
                .Where(m => !m.Matched && !marcasExistentes.Contains(m.MarcaId))
                .Select(m => new MarcaCore(m.Nombre) { Id = m.MarcaId })
                .ToList();
        }

        private List<ProductoCore> GetProductosNuevosBySupermercado(
            List<ProductoProcesadoDto> productosProcesados,
            HashSet<int> productosExistentes,
            int categoriaId)
        {
            return productosProcesados
                .Where(p => !p.Matched &&
                           p.ProductoId.HasValue &&
                           !productosExistentes.Contains(p.ProductoId.Value))
                .Select(p => new ProductoCore(p.Nombre, p.MarcaId ?? 0, categoriaId)
                {
                    Id = p.ProductoId.Value,
                    ImgUrl = p.Imagen
                })
                .ToList();
        }

        // ✅ NUEVO: Precios históricos para productos MATCHED
        private List<PrecioHistoricoCore> GetPreciosHistoricosProductosMatched(
            List<ProductoProcesadoDto> productosProcesados,
            int supermercadoId)
        {
            var productosMatched = productosProcesados
                .Where(p => p.Matched && p.ProductoId.HasValue && p.Precio > 0)
                .ToList();

            if (!productosMatched.Any())
            {
                Console.WriteLine($"[INFO] No hay productos matched para supermercado {supermercadoId}");
                return new List<PrecioHistoricoCore>();
            }

            Console.WriteLine($"[INFO] Procesando {productosMatched.Count} productos MATCHED para supermercado {supermercadoId}");

            var preciosUnicos = new Dictionary<int, PrecioHistoricoCore>();

            foreach (var producto in productosMatched)
            {
                var productoId = producto.ProductoId.Value;

                // Para productos matched, el último precio gana (sobrescribir duplicados)
                preciosUnicos[productoId] = new PrecioHistoricoCore(
                    productoId,
                    supermercadoId,
                    producto.Precio);
            }

            Console.WriteLine($"[INFO] Creados {preciosUnicos.Count} precios históricos únicos para productos MATCHED");
            return preciosUnicos.Values.ToList();
        }

        // ✅ RENOMBRADO: Precios históricos para productos NUEVOS solamente
        private List<PrecioHistoricoCore> GetPreciosHistoricosProductosNuevos(
            List<ProductoProcesadoDto> productosProcesados,
            int supermercadoId)
        {
            var productosNuevos = productosProcesados
                .Where(p => !p.Matched && p.ProductoId.HasValue && p.Precio > 0)
                .ToList();

            if (!productosNuevos.Any())
            {
                Console.WriteLine($"[INFO] No hay productos NUEVOS para supermercado {supermercadoId}");
                return new List<PrecioHistoricoCore>();
            }

            Console.WriteLine($"[INFO] Procesando {productosNuevos.Count} productos NUEVOS para supermercado {supermercadoId}");

            var preciosUnicos = new Dictionary<int, PrecioHistoricoCore>();

            foreach (var producto in productosNuevos)
            {
                var productoId = producto.ProductoId.Value;

                if (!preciosUnicos.ContainsKey(productoId))
                {
                    preciosUnicos[productoId] = new PrecioHistoricoCore(
                        productoId,
                        supermercadoId,
                        producto.Precio);
                }
            }

            Console.WriteLine($"[INFO] Creados {preciosUnicos.Count} precios históricos únicos para productos NUEVOS");
            return preciosUnicos.Values.ToList();
        }

        #endregion
    }
}