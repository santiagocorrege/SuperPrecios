using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class MatchingRepositoryEF : IMatchingRepository
    {
        private readonly SuperPreciosDbContext _context;

        public MatchingRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        // ✅ CORREGIDO: Compatible con SqlServerRetryingExecutionStrategy
        public async Task ProcessMatchingTransactionAsync(
            IEnumerable<Marca> marcasNuevas,
            IEnumerable<Producto> productosNuevos,
            IEnumerable<PrecioHistorico> preciosHistoricos)
        {
            Console.WriteLine("[INFO] Iniciando transacción de matching...");

            // ✅ SOLUCIÓN: Usar ExecutionStrategy para manejar transacciones + retry
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // ✅ TRANSACCIÓN DENTRO DEL EXECUTION STRATEGY
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Configurar contexto para operaciones masivas
                    _context.ConfigureForBulkOperations();

                    // ✅ ORDEN IMPORTANTE: Marcas → Productos → Precios (por FKs)

                    var marcasArray = marcasNuevas.ToArray();
                    if (marcasArray.Any())
                    {
                        Console.WriteLine($"[INFO] Insertando {marcasArray.Length} marcas...");
                        await _context.Marcas.AddRangeAsync(marcasArray);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[SUCCESS] {marcasArray.Length} marcas insertadas");
                    }

                    var productosArray = productosNuevos.ToArray();
                    if (productosArray.Any())
                    {
                        Console.WriteLine($"[INFO] Insertando {productosArray.Length} productos...");
                        await _context.Productos.AddRangeAsync(productosArray);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[SUCCESS] {productosArray.Length} productos insertados");
                    }

                    var preciosArray = preciosHistoricos.ToArray();
                    if (preciosArray.Any())
                    {
                        Console.WriteLine($"[INFO] Insertando {preciosArray.Length} precios históricos...");
                        await _context.PreciosHistoricos.AddRangeAsync(preciosArray);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[SUCCESS] {preciosArray.Length} precios históricos insertados");
                    }

                    // ✅ COMMIT: Todo exitoso
                    await transaction.CommitAsync();
                    Console.WriteLine("[SUCCESS] Transacción de matching completada exitosamente");
                }
                catch (Exception ex)
                {
                    // ✅ ROLLBACK AUTOMÁTICO: Si algo falla, se revierte TODO
                    await transaction.RollbackAsync();
                    Console.WriteLine($"[ERROR] Error en transacción - ROLLBACK ejecutado: {ex.Message}");
                    throw;
                }
                finally
                {
                    // Restaurar configuración normal del contexto
                    _context.RestoreNormalConfiguration();
                }
            });
        }

        public async Task<HashSet<int>> GetMarcasExistentesByIdsAsync(IEnumerable<int> marcasIds)
        {
            var idsArray = marcasIds.ToArray();
            if (!idsArray.Any()) return new HashSet<int>();

            var marcasExistentes = await _context.Marcas
                .AsNoTracking()
                .Where(m => idsArray.Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync();

            return marcasExistentes.ToHashSet();
        }

        public async Task<HashSet<int>> GetProductosExistentesByIdsAsync(IEnumerable<int> productosIds)
        {
            var idsArray = productosIds.ToArray();
            if (!idsArray.Any()) return new HashSet<int>();

            var productosExistentes = await _context.Productos
                .AsNoTracking()
                .Where(p => idsArray.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            return productosExistentes.ToHashSet();
        }

        public async Task ValidateSupermercadosAndCategoriasAsync(IEnumerable<int> supermercadosIds, IEnumerable<int> categoriasIds)
        {
            var supermercadosArray = supermercadosIds.ToArray();
            var categoriasArray = categoriasIds.ToArray();

            // Validar supermercados en batch
            var supermercadosExistentes = await _context.Supermercados
                .AsNoTracking()
                .Where(s => supermercadosArray.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var supermercadosFaltantes = supermercadosArray.Except(supermercadosExistentes).ToArray();
            if (supermercadosFaltantes.Any())
                throw new ArgumentException($"Supermercados no existen: {string.Join(", ", supermercadosFaltantes)}");

            // Validar categorías en batch
            var categoriasExistentes = await _context.Categorias
                .AsNoTracking()
                .Where(c => categoriasArray.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            var categoriasFaltantes = categoriasArray.Except(categoriasExistentes).ToArray();
            if (categoriasFaltantes.Any())
                throw new ArgumentException($"Categorías no existen: {string.Join(", ", categoriasFaltantes)}");

            Console.WriteLine($"[INFO] Validación exitosa - Supermercados: {supermercadosExistentes.Count}, Categorías: {categoriasExistentes.Count}");
        }
    }
}