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

            foreach (var resultado in matchingResults.Resultados)
            {
                // Validar supermercado y categoría
                SupermercadoCore supermercado = await _supermercadoRepository.GetByIdAsync(resultado.Supermercado.Id);
                if (supermercado == null)
                    throw new ArgumentException($"Supermercado con ID {resultado.Supermercado.Id} no existe");

                CategoriaCore categoria = await _categoriaRepository.GetByIdAsync(resultado.Ruta.Id);
                if (categoria == null)
                    throw new ArgumentException($"Categoría con ID {resultado.Ruta.Id} no existe");

                // FASE 1: Crear marcas nuevas
                await ProcesarMarcasNuevasAsync(resultado.MarcasProcesadas);

                // FASE 2: Procesar productos y crear precios históricos inmediatamente
                await ProcesarProductosYPreciosHistoricosAsync(resultado.ProductosProcesados, categoria, supermercado);
            }
        }

        /// <summary>
        /// FASE 1: Crear solo las marcas que no fueron matcheadas (nuevas)
        /// </summary>
        private async Task ProcesarMarcasNuevasAsync(List<MarcaProcesadaDto> marcasProcesadas)
        {
            if (marcasProcesadas == null || !marcasProcesadas.Any())
                return;

            foreach (var marcaDto in marcasProcesadas.Where(m => !m.Matched))
            {
                try
                {
                    // Verificar si la marca ya existe (por seguridad)
                    var marcaExistente = await GetMarcaByNombreAsync(marcaDto.Nombre);
                    if (marcaExistente == null)
                    {
                        // Crear nueva marca
                        var nuevaMarca = new MarcaCore(marcaDto.Nombre);
                        await AddMarcaAsync(nuevaMarca);

                        Console.WriteLine($"[INFO] Marca nueva creada: {marcaDto.Nombre} (ID Python: {marcaDto.MarcaId})");
                    }
                    else
                    {
                        Console.WriteLine($"[WARN] Marca {marcaDto.Nombre} ya existía en BD, no se creó duplicado");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error creando marca {marcaDto.Nombre}: {ex.Message}");
                    // Continuar con otras marcas
                }
            }
        }

        /// <summary>
        /// FASE 2: Procesar productos y crear precios históricos inmediatamente
        /// </summary>
        private async Task ProcesarProductosYPreciosHistoricosAsync(
            List<ProductoProcesadoDto> productosProcesados,
            CategoriaCore categoria,
            SupermercadoCore supermercado)
        {
            var preciosHistoricos = new List<PrecioHistoricoCore>();

            foreach (var productoDto in productosProcesados)
            {
                try
                {
                    ProductoCore producto = null;

                    if (productoDto.Matched && productoDto.ProductoId.HasValue)
                    {
                        // Producto existente - buscar por ID si está disponible
                        try
                        {
                            producto = await GetProductoByIdAsync(productoDto.ProductoId.Value);
                        }
                        catch (KeyNotFoundException)
                        {
                            // Si no existe por ID, buscar por nombre como fallback
                            try
                            {
                                producto = await GetProductoByNombreAsync(productoDto.Nombre);
                            }
                            catch (KeyNotFoundException)
                            {
                                // Si tampoco existe por nombre, crear nuevo
                                producto = await CreateProductoAsync(productoDto, categoria);
                            }
                        }
                    }
                    else
                    {
                        // Producto nuevo
                        producto = await CreateProductoAsync(productoDto, categoria);
                    }

                    if (producto != null)
                    {
                        // Crear precio histórico inmediatamente
                        var precioHistorico = new PrecioHistoricoCore(producto, supermercado, productoDto.Precio);
                        preciosHistoricos.Add(precioHistorico);

                        Console.WriteLine($"[INFO] Producto procesado: {producto.Nombre} - Precio: {productoDto.Precio}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error procesando producto {productoDto.Nombre}: {ex.Message}");
                    continue;
                }
            }

            // Guardar todos los precios históricos en lote
            if (preciosHistoricos.Any())
            {
                await AddPreciosHistoricosBySupermercadoAndCategoriaAsync(
                    preciosHistoricos, supermercado, categoria);

                Console.WriteLine($"[INFO] Guardados {preciosHistoricos.Count} precios históricos para {supermercado.Nombre}");
            }
        }

        /// <summary>
        /// Crear nuevo producto con marca resuelta
        /// </summary>
        private async Task<ProductoCore> CreateProductoAsync(ProductoProcesadoDto productoDto, CategoriaCore categoria)
        {
            // Buscar marca (existente o recién creada en FASE 1)
            MarcaCore marca = null;

            if (productoDto.MarcaId.HasValue)
            {
                // Intentar buscar por el nombre de marca del DTO
                string nombreMarca = productoDto.Marca ?? "Sin Marca";
                marca = await GetMarcaByNombreAsync(nombreMarca);
            }

            if (marca == null)
            {
                // Si no se encuentra marca, crear una nueva como fallback
                string nombreMarca = productoDto.Marca ?? "Sin Marca";
                marca = new MarcaCore(nombreMarca);
                await AddMarcaAsync(marca);
                Console.WriteLine($"[WARN] Marca creada como fallback: {nombreMarca}");
            }

            // Crear producto
            var producto = new ProductoCore(productoDto.Nombre, marca, categoria, productoDto.Imagen);
            await AddProductoAsync(producto);

            Console.WriteLine($"[INFO] Producto creado: {producto.Nombre} con marca {marca.Nombre}");
            return producto;
        }

        // ===== MÉTODOS DE ACCESO A REPOSITORIOS CON NOMENCLATURA CONSISTENTE =====

        /// <summary>
        /// Get marca by nombre de forma segura
        /// </summary>
        private async Task<MarcaCore> GetMarcaByNombreAsync(string nombreMarca)
        {
            try
            {
                var marcaEntity = new MarcaCore(nombreMarca);
                return await _marcaRepository.GetByNombreAsync(marcaEntity);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARN] Error en GetMarcaByNombre {nombreMarca}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Add marca async
        /// </summary>
        private async Task AddMarcaAsync(MarcaCore marca)
        {
            await _marcaRepository.AddAsync(marca);
        }

        /// <summary>
        /// Get producto by ID async
        /// </summary>
        private async Task<ProductoCore> GetProductoByIdAsync(int id)
        {
            return await _productoRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Get producto by nombre async
        /// </summary>
        private async Task<ProductoCore> GetProductoByNombreAsync(string nombre)
        {
            return await _productoRepository.GetByNombreAsync(nombre);
        }

        /// <summary>
        /// Add producto async
        /// </summary>
        private async Task AddProductoAsync(ProductoCore producto)
        {
            await _productoRepository.AddAsync(producto);
        }

        /// <summary>
        /// Add precios históricos by supermercado and categoría async
        /// </summary>
        private async Task AddPreciosHistoricosBySupermercadoAndCategoriaAsync(
            IEnumerable<PrecioHistoricoCore> preciosHistoricos,
            SupermercadoCore supermercado,
            CategoriaCore categoria)
        {
            await _precioHistoricoRepository.AddAsyncBySupermercadoAndCategoria(preciosHistoricos, supermercado, categoria);
        }
    }
}