using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System.Data.Common;

namespace SuperPrecios.Infrastructure.EF
{
    public class PrecioHistoricoRepositoryEF : IPrecioHistoricoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public PrecioHistoricoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        // ✅ NUEVO: Método de alta performance para IDs sincronizados
        public async Task AddAsyncBulkWithSyncedIds(IEnumerable<PrecioHistorico> preciosHistoricos)
        {
            if (preciosHistoricos == null || !preciosHistoricos.Any())
            {
                Console.WriteLine("[INFO] No hay precios históricos para procesar");
                return;
            }

            var preciosArray = preciosHistoricos.ToArray();
            var productosIds = preciosArray.Select(p => p.ProductoId).Distinct().ToArray();
            var supermercadosIds = preciosArray.Select(p => p.SupermercadoId).Distinct().ToArray();

            Console.WriteLine($"[INFO] Procesando {preciosArray.Length} precios históricos...");

            try
            {
                // ✅ OPTIMIZACIÓN 1: Una sola consulta para validar todos los productos
                var productosExistentes = await _context.Productos
                    .Where(p => productosIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToHashSetAsync(); // HashSet para búsquedas O(1)

                // ✅ OPTIMIZACIÓN 2: Una sola consulta para validar todos los supermercados
                var supermercadosExistentes = await _context.Supermercados
                    .Where(s => supermercadosIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToHashSetAsync();

                // ✅ OPTIMIZACIÓN 3: Filtrar precios válidos en memoria
                var preciosValidos = preciosArray
                    .Where(p => productosExistentes.Contains(p.ProductoId) &&
                               supermercadosExistentes.Contains(p.SupermercadoId))
                    .ToList();

                if (!preciosValidos.Any())
                {
                    Console.WriteLine("[WARN] No hay precios históricos válidos para insertar");
                    return;
                }

                Console.WriteLine($"[INFO] {preciosValidos.Count} de {preciosArray.Length} precios son válidos");

                // ✅ OPTIMIZACIÓN 4: Inserción en lote con manejo de duplicados
                await InsertarEnLoteConManejoDuplicados(preciosValidos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en AddAsyncBulkWithSyncedIds: {ex.Message}");
                throw;
            }
        }

        private async Task InsertarEnLoteConManejoDuplicados(List<PrecioHistorico> preciosValidos)
        {
            try
            {
                // ✅ OPTIMIZACIÓN 5: Inserción masiva usando AddRange
                await _context.PreciosHistoricos.AddRangeAsync(preciosValidos);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[SUCCESS] Insertados {preciosValidos.Count} precios históricos en lote");
            }
            catch (DbUpdateException dbEx)
            {
                // Si falla la inserción en lote, usar estrategia de recuperación
                Console.WriteLine($"[WARN] Fallo inserción en lote, usando inserción individual con filtro de duplicados...");
                await InsertarIndividualConFiltroDuplicados(preciosValidos, dbEx);
            }
        }

        private async Task InsertarIndividualConFiltroDuplicados(List<PrecioHistorico> preciosValidos, DbUpdateException originalException)
        {
            // ✅ OPTIMIZACIÓN 6: Consultar duplicados existentes para evitar errores
            var fechasExistentes = preciosValidos.Select(p => p.Fecha).Distinct().ToArray();
            var productosIds = preciosValidos.Select(p => p.ProductoId).ToArray();
            var supermercadosIds = preciosValidos.Select(p => p.SupermercadoId).ToArray();

            var duplicadosExistentes = await _context.PreciosHistoricos
                .Where(ph => productosIds.Contains(ph.ProductoId) &&
                            supermercadosIds.Contains(ph.SupermercadoId) &&
                            fechasExistentes.Contains(ph.Fecha))
                .Select(ph => new { ph.ProductoId, ph.SupermercadoId, ph.Fecha })
                .ToHashSetAsync();

            var preciosSinDuplicados = preciosValidos
                .Where(p => !duplicadosExistentes.Contains(new { p.ProductoId, p.SupermercadoId, p.Fecha }))
                .ToList();

            Console.WriteLine($"[INFO] Filtrando duplicados: {preciosValidos.Count - preciosSinDuplicados.Count} duplicados encontrados");

            if (!preciosSinDuplicados.Any())
            {
                Console.WriteLine("[INFO] Todos los precios ya existen, no hay nada que insertar");
                return;
            }

            // Insertar los precios sin duplicados
            var insertadosExitosos = 0;
            var errores = 0;

            foreach (var precio in preciosSinDuplicados)
            {
                try
                {
                    await _context.PreciosHistoricos.AddAsync(precio);
                    await _context.SaveChangesAsync();
                    insertadosExitosos++;
                }
                catch (DbUpdateException individualEx)
                {
                    errores++;
                    if (individualEx.InnerException is SqlException sqlEx)
                    {
                        if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                        {
                            Console.WriteLine($"[WARN] Duplicado: ProductoID {precio.ProductoId} - SupermercadoID {precio.SupermercadoId} - Fecha {precio.Fecha}");
                            continue;
                        }
                        if (sqlEx.Number == 547) // Foreign key violation
                        {
                            Console.WriteLine($"[WARN] FK violation: ProductoID {precio.ProductoId} o SupermercadoID {precio.SupermercadoId} no existe");
                            continue;
                        }
                    }
                    Console.WriteLine($"[ERROR] Error insertando precio individual: {individualEx.Message}");
                }
                catch (Exception ex)
                {
                    errores++;
                    Console.WriteLine($"[ERROR] Error inesperado: {ex.Message}");
                }
            }

            Console.WriteLine($"[SUMMARY] Inserción completada - Exitosos: {insertadosExitosos}, Errores: {errores}");
        }

        // ===== MÉTODOS EXISTENTES (mantenidos para compatibilidad) =====

        public async Task AddAsync(PrecioHistorico precioHistorico)
        {
            // Implementación existente...
            if (precioHistorico == null)
                throw new ArgumentNullException(nameof(precioHistorico), "El precio histórico no puede ser nulo");

            try
            {
                await _context.PreciosHistoricos.AddAsync(precioHistorico);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        throw new Exception("Error de duplicado en la tabla");
                    if (sqlEx.Number == 547)
                        throw new Exception("Violación de clave foránea");
                }
                throw new Exception("Error al guardar el precio histórico en la base de datos.");
            }
        }

        public async Task AddAsyncBySupermercadoAndCategoria(IEnumerable<PrecioHistorico> preciosHistoricos, Supermercado supermercado, Categoria categoria)
        {
            // Para mantener compatibilidad, delegar al nuevo método optimizado
            Console.WriteLine("[INFO] Usando método optimizado para AddAsyncBySupermercadoAndCategoria");
            await AddAsyncBulkWithSyncedIds(preciosHistoricos);
        }

        public async Task<IEnumerable<Producto>> GetAllBySupermercado(int supermercadoId)
        {
            if (supermercadoId <= 0) throw new ArgumentException("El id del supermercado no puede ser menor o igual a 0");
            try
            {
                var super = await _context.Supermercados.FindAsync(supermercadoId);
                if (super == null) throw new ArgumentNullException("El Supermercado no existe");
                var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.PreciosHistoricos.Where(ph => ph.SupermercadoId == supermercadoId))
                .ToListAsync();
                return productos;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, int productoId)
        {
            if (productoId <= 0 || supermercadoId <= 0) throw new ArgumentException("Id de producto y/o supermercado invalido");
            try
            {
                var super = await _context.Supermercados.FindAsync(supermercadoId);
                if (super == null) throw new ArgumentNullException("El Supermercado no existe");
                return await _context.PreciosHistoricos
                         .AsNoTracking()
                         .Where(ph => ph.Producto.Id == productoId
                                   && ph.SupermercadoId == supermercadoId)
                         .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Error al buscar el precio historico del producto en la base de datos.", dbEx);
            }
        }

        public async Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, string productoNombre)
        {
            if (String.IsNullOrWhiteSpace(productoNombre) || supermercadoId <= 0) throw new ArgumentException("Id de producto y/o nombre supermercado invalido");
            try
            {
                var super = await _context.Supermercados.FindAsync(supermercadoId);
                if (super == null) throw new ArgumentNullException("El Supermercado no existe");
                return await _context.PreciosHistoricos
                         .AsNoTracking()
                         .Where(ph =>
                         ph.Producto.Nombre == productoNombre &&
                         ph.SupermercadoId == supermercadoId
                         )
                         .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Error al buscar el supermercado en la base de datos.", dbEx);
            }
        }
    }
}