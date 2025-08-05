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
        private readonly IProductoRepository _productoRepository;
        private readonly IMarcaRepository _marcaRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ISupermercadoRepository _supermercadoRepository;
        private readonly IPrecioHistoricoRepository _precioHistoricoRepository;

        public MatchingProcessService(
            IProductoRepository productoRepository,
            IMarcaRepository marcaRepository,
            ICategoriaRepository categoriaRepository,
            ISupermercadoRepository supermercadoRepository,
            IPrecioHistoricoRepository precioHistoricoRepository)
        {
            _productoRepository = productoRepository;
            _marcaRepository = marcaRepository;
            _categoriaRepository = categoriaRepository;
            _supermercadoRepository = supermercadoRepository;
            _precioHistoricoRepository = precioHistoricoRepository;
        }

        public async Task ProcessMatchingResultsAsync(MiniPssResultadoDto matchingResults)
        {
            if (matchingResults?.Resultados == null)
                throw new ArgumentException("Los resultados del matching no pueden ser nulos");

            Console.WriteLine($"[INFO] Iniciando procesamiento SECUENCIAL de {matchingResults.Resultados.Count} resultados de matching");
            var tiempoInicio = DateTime.Now;

            // ✅ CRÍTICO: Procesar UN resultado a la vez (un supermercado a la vez)
            foreach (var resultado in matchingResults.Resultados)
            {
                try
                {
                    Console.WriteLine($"[INFO] === PROCESANDO SUPERMERCADO {resultado.Supermercado.Id} ===");

                    // Validar supermercado y categoría
                    SupermercadoCore supermercado = await _supermercadoRepository.GetByIdAsync(resultado.Supermercado.Id);
                    if (supermercado == null)
                        throw new ArgumentException($"Supermercado con ID {resultado.Supermercado.Id} no existe");

                    CategoriaCore categoria = await _categoriaRepository.GetByIdAsync(resultado.Ruta.Id);
                    if (categoria == null)
                        throw new ArgumentException($"Categoría con ID {resultado.Ruta.Id} no existe");

                    Console.WriteLine($"[INFO] Procesando {resultado.ProductosProcesados.Count} productos para {supermercado.Nombre}");

                    // ✅ PASO 1: Procesar TODAS las marcas de este supermercado
                    await ProcesarTodasLasMarcasDelSupermercado(resultado.MarcasProcesadas);

                    // ✅ PASO 2: Procesar TODOS los productos de este supermercado  
                    await ProcesarTodosLosProductosDelSupermercado(resultado.ProductosProcesados, categoria);

                    // ✅ PASO 3: Procesar TODOS los precios históricos de este supermercado
                    await ProcesarTodosLosPreciosDelSupermercado(resultado.ProductosProcesados, supermercado);

                    Console.WriteLine($"[SUCCESS] Supermercado {supermercado.Nombre} procesado completamente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error procesando supermercado {resultado.Supermercado.Id}: {ex.Message}");
                    // ✅ DECIDIR: ¿Continuar con otros supermercados o fallar todo?
                    // Para robustez, continuamos con otros supermercados
                    continue;
                }
            }

            var tiempoTotal = DateTime.Now - tiempoInicio;
            Console.WriteLine($"[SUCCESS] Procesamiento COMPLETO terminado en {tiempoTotal.TotalSeconds:F2} segundos");
        }

        /// <summary>
        /// Procesa TODAS las marcas de un supermercado de forma completamente secuencial
        /// </summary>
        private async Task ProcesarTodasLasMarcasDelSupermercado(List<MarcaProcesadaDto> marcasProcesadas)
        {
            if (marcasProcesadas == null || !marcasProcesadas.Any())
                return;

            var marcasNuevas = marcasProcesadas.Where(m => !m.Matched).ToList();

            if (!marcasNuevas.Any())
            {
                Console.WriteLine("[INFO] No hay marcas nuevas para procesar");
                return;
            }

            Console.WriteLine($"[INFO] Procesando {marcasNuevas.Count} marcas nuevas SECUENCIALMENTE...");

            // ✅ COMPLETAMENTE SECUENCIAL: Una marca a la vez, esperando completamente
            foreach (var marcaDto in marcasNuevas)
            {
                try
                {
                    // Verificar si ya existe ANTES de intentar crear
                    MarcaCore marcaExistente = null;
                    try
                    {
                        marcaExistente = await _marcaRepository.GetByIdAsync(marcaDto.MarcaId);
                    }
                    catch (KeyNotFoundException)
                    {
                        // No existe, está bien
                    }

                    if (marcaExistente != null)
                    {
                        Console.WriteLine($"[WARN] Marca ID {marcaDto.MarcaId} ya existe, saltando...");
                        continue;
                    }

                    // Crear marca nueva
                    var nuevaMarca = new MarcaCore(marcaDto.Nombre)
                    {
                        Id = marcaDto.MarcaId
                    };

                    await _marcaRepository.AddAsync(nuevaMarca);
                    Console.WriteLine($"[DEBUG] Marca creada: ID={marcaDto.MarcaId}, Nombre={marcaDto.Nombre}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error creando marca {marcaDto.Nombre} (ID: {marcaDto.MarcaId}): {ex.Message}");
                    // Continuar con otras marcas
                }
            }

            Console.WriteLine($"[SUCCESS] Marcas del supermercado procesadas");
        }

        /// <summary>
        /// Procesa TODOS los productos de un supermercado de forma completamente secuencial
        /// </summary>
        private async Task ProcesarTodosLosProductosDelSupermercado(List<ProductoProcesadoDto> productosProcesados, CategoriaCore categoria)
        {
            if (productosProcesados == null || !productosProcesados.Any())
                return;

            var productosNuevos = productosProcesados.Where(p => !p.Matched && p.ProductoId.HasValue).ToList();

            if (!productosNuevos.Any())
            {
                Console.WriteLine("[INFO] No hay productos nuevos para procesar");
                return;
            }

            Console.WriteLine($"[INFO] Procesando {productosNuevos.Count} productos nuevos SECUENCIALMENTE...");

            // ✅ COMPLETAMENTE SECUENCIAL: Un producto a la vez, esperando completamente
            foreach (var productoDto in productosNuevos)
            {
                try
                {
                    // Verificar si ya existe ANTES de intentar crear
                    ProductoCore productoExistente = null;
                    try
                    {
                        productoExistente = await _productoRepository.GetByIdAsync(productoDto.ProductoId.Value);
                    }
                    catch (KeyNotFoundException)
                    {
                        // No existe, está bien
                    }

                    if (productoExistente != null)
                    {
                        Console.WriteLine($"[WARN] Producto ID {productoDto.ProductoId} ya existe, saltando...");
                        continue;
                    }

                    // Crear producto nuevo
                    var nuevoProducto = new ProductoCore(
                        productoDto.Nombre,
                        productoDto.MarcaId ?? 0,
                        categoria.Id)
                    {
                        Id = productoDto.ProductoId.Value,
                        ImgUrl = productoDto.Imagen
                    };

                    await _productoRepository.AddAsync(nuevoProducto);
                    Console.WriteLine($"[DEBUG] Producto creado: ID={productoDto.ProductoId}, Nombre={productoDto.Nombre}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error creando producto {productoDto.Nombre} (ID: {productoDto.ProductoId}): {ex.Message}");
                    // Continuar con otros productos
                }
            }

            Console.WriteLine($"[SUCCESS] Productos del supermercado procesados");
        }

        /// <summary>
        /// Procesa TODOS los precios históricos de un supermercado usando el método optimizado
        /// </summary>
        private async Task ProcesarTodosLosPreciosDelSupermercado(List<ProductoProcesadoDto> productosProcesados, SupermercadoCore supermercado)
        {
            if (productosProcesados == null || !productosProcesados.Any())
                return;

            Console.WriteLine($"[INFO] Preparando precios históricos para {supermercado.Nombre}...");

            // ✅ Crear precios históricos únicos (evitar duplicados)
            var preciosHistoricosMap = new Dictionary<(int ProductoId, int SupermercadoId), PrecioHistoricoCore>();

            foreach (var productoDto in productosProcesados)
            {
                try
                {
                    int productoId = productoDto.ProductoId ?? 0;

                    if (productoId <= 0)
                    {
                        Console.WriteLine($"[WARN] ProductoId inválido para {productoDto.Nombre}");
                        continue;
                    }

                    if (productoDto.Precio <= 0)
                    {
                        Console.WriteLine($"[WARN] Precio inválido ({productoDto.Precio}) para producto {productoDto.Nombre}");
                        continue;
                    }

                    var key = (productoId, supermercado.Id);

                    if (!preciosHistoricosMap.ContainsKey(key))
                    {
                        var precioHistorico = new PrecioHistoricoCore(
                            productoId,
                            supermercado.Id,
                            productoDto.Precio);

                        preciosHistoricosMap[key] = precioHistorico;
                    }
                    else
                    {
                        Console.WriteLine($"[WARN] Precio histórico duplicado ignorado para ProductoId={productoId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error preparando precio histórico para producto {productoDto.Nombre}: {ex.Message}");
                    continue;
                }
            }

            var preciosHistoricosUnicos = preciosHistoricosMap.Values.ToList();

            if (preciosHistoricosUnicos.Any())
            {
                try
                {
                    var tiempoInicio = DateTime.Now;

                    // ✅ Esta operación es thread-safe y optimizada
                    await _precioHistoricoRepository.AddAsyncBulkWithSyncedIds(preciosHistoricosUnicos);

                    var tiempoInsercion = DateTime.Now - tiempoInicio;
                    Console.WriteLine($"[SUCCESS] {preciosHistoricosUnicos.Count} precios históricos procesados para {supermercado.Nombre} en {tiempoInsercion.TotalSeconds:F2}s");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error guardando precios históricos para {supermercado.Nombre}: {ex.Message}");
                    throw;
                }
            }
            else
            {
                Console.WriteLine("[WARN] No hay precios históricos válidos para procesar");
            }
        }
    }
}