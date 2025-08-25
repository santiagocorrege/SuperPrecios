using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class PrecioHistoricoRepositoryEF : IPrecioHistoricoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public PrecioHistoricoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        public async Task<List<PrecioHistorico>> GetPreciosHistoricosBySupermercado(
            int supermercadoId,
            List<Producto> productos)
        {
            // ✅ CORREGIDO: Fecha actual real
            DateOnly hoy = TimeHelper.DateOnlyNowInMontevideo();

            if (productos == null || !productos.Any())
                throw new ArgumentException("La lista de productos no puede estar vacía");

            try
            {
                // ✅ CORREGIDO: Obtener IDs de productos para la consulta
                var productosIds = productos.Select(p => p.Id).ToList();

                // ✅ CORREGIDO: Consulta LINQ estructurada correctamente
                var preciosHistoricos = await _context.PreciosHistoricos
                    .AsNoTracking()
                    .Where(ph => ph.SupermercadoId == supermercadoId &&
                                ph.Fecha == hoy &&
                                productosIds.Contains(ph.ProductoId))
                    .Include(ph => ph.Producto)
                        .ThenInclude(p => p.Marca)
                    .Include(ph => ph.Producto)
                        .ThenInclude(p => p.Categoria)
                    .Include(ph => ph.Supermercado)
                    .ToListAsync();

                // ✅ CORREGIDO: Validar disponibilidad completa
                var productosEncontrados = preciosHistoricos.Select(ph => ph.ProductoId).Distinct().Count();
                if (productosEncontrados != productos.Count)
                {
                    var productosNoEncontrados = productosIds
                        .Except(preciosHistoricos.Select(ph => ph.ProductoId))
                        .ToList();

                    throw new InvalidOperationException(
                        $"No existen precios para los productos con IDs: {string.Join(", ", productosNoEncontrados)} " +
                        $"en el supermercado {supermercadoId} para la fecha {hoy}");
                }

                return preciosHistoricos;
            }
            catch (DbException ex)
            {
                throw new Exception($"Error en la base de datos al consultar precios históricos: {ex.Message}", ex);
            }
        }

        public async Task<List<Supermercado>> GetSupermercadosFilterPreciosHistoricos(List<Producto> productos)
        {
            // ✅ FECHA ACTUAL CORRECTA
            DateOnly hoy = TimeHelper.DateOnlyNowInMontevideo();

            if (productos == null || !productos.Any())
                throw new ArgumentException("La lista de productos no puede estar vacía");

            var productosIds = productos.Select(p => p.Id).Distinct().ToList();
            var cantidadProductosSolicitados = productosIds.Count;

            try
            {
                Console.WriteLine($"[INFO] Buscando supermercados con disponibilidad completa para {cantidadProductosSolicitados} productos en fecha {hoy}");

                // ✅ PASO 1: Encontrar supermercados que tengan TODOS los productos
                var supermercadosCompletos = await _context.PreciosHistoricos
                    .AsNoTracking()
                    .Where(ph => ph.Fecha == hoy && productosIds.Contains(ph.ProductoId))
                    .GroupBy(ph => ph.SupermercadoId)
                    .Where(grupo => grupo.Select(ph => ph.ProductoId).Distinct().Count() == cantidadProductosSolicitados)
                    .Select(grupo => grupo.Key)
                    .ToListAsync();

                if (!supermercadosCompletos.Any())
                {
                    Console.WriteLine("[WARN] No se encontraron supermercados con disponibilidad completa");
                    return new List<Supermercado>();
                }

                Console.WriteLine($"[INFO] Encontrados {supermercadosCompletos.Count} supermercados con disponibilidad completa");

                // ✅ PASO 2: Obtener supermercados completos con sus precios históricos
                var supermercados = await _context.Supermercados
                    .AsNoTracking()
                    .Where(s => supermercadosCompletos.Contains(s.Id))                                        
                    .ToListAsync();

                Console.WriteLine($"[SUCCESS] Retornando {supermercados.Count} supermercados con precios completos");

                return supermercados;
            }
            catch (DbException ex)
            {
                Console.WriteLine($"[ERROR] Error en consulta: {ex.Message}");
                throw new Exception("Error al consultar supermercados con precios históricos", ex);
            }
        }
    }
}
