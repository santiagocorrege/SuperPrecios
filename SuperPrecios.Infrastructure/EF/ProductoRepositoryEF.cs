using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.Common;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Shared;
using System.Data.Common;

namespace SuperPrecios.Infrastructure.EF
{
    public class ProductoRepositoryEF : IProductoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public ProductoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        // ✅ MÉTODO OPTIMIZADO PARA IDS SINCRONIZADOS
        public async Task AddAsync(Producto producto)
        {
            try
            {
                // ✅ VALIDACIÓN: ID debe ser proporcionado por Python
                if (producto.Id <= 0)
                    throw new ArgumentException("El ID del producto debe ser proporcionado por el sistema externo (Python)");

                // ✅ VERIFICAR: Si el producto ya existe
                var productoExistente = await _context.Productos.FindAsync(producto.Id);
                if (productoExistente != null)
                {
                    throw new ArgumentException($"El producto con ID {producto.Id} ya existe en la base de datos.");
                }

                // ✅ SECUENCIAL: Validar FK de forma secuencial (no paralela)
                var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == producto.CategoriaId);
                if (!categoriaExiste)
                {
                    throw new ArgumentException($"La categoría con ID {producto.CategoriaId} no existe.");
                }

                var marcaExiste = await _context.Marcas.AnyAsync(m => m.Id == producto.MarcaId);
                if (!marcaExiste)
                {
                    throw new ArgumentException($"La marca con ID {producto.MarcaId} no existe.");
                }

                // ✅ INSERCIÓN
                await _context.Productos.AddAsync(producto);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[DEBUG] Producto agregado exitosamente: ID={producto.Id}, Nombre={producto.Nombre}");
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    SqlException sqlException = dbEx.InnerException as SqlException;
                    if (sqlException.Number == 2627) // Unique constraint error
                    {
                        throw new Exception($"Error: El producto con ID {producto.Id} ya existe en la base de datos.");
                    }
                    if (sqlException.Number == 547) // Foreign key violation
                    {
                        throw new Exception("Error: El producto no puede ser agregado debido a una violación de clave foránea (MarcaId o CategoriaId inválidos).");
                    }
                }
                throw new Exception("Error al agregar el producto a la base de datos.", dbEx);
            }
        }

        // ✅ MÉTODO ADICIONAL: Inserción en lote para alta performance
        public async Task AddRangeAsync(IEnumerable<Producto> productos)
        {
            if (productos == null || !productos.Any())
                return;

            var productosArray = productos.ToArray();
            Console.WriteLine($"[INFO] Insertando {productosArray.Length} productos en lote...");

            try
            {
                // Validar todos los IDs de una vez
                var productosIds = productosArray.Select(p => p.Id).ToArray();
                var marcasIds = productosArray.Select(p => p.MarcaId).Distinct().ToArray();
                var categoriasIds = productosArray.Select(p => p.CategoriaId).Distinct().ToArray();

                // Verificar existencia de dependencias en paralelo
                var productosExistentes = _context.Productos
                    .Where(p => productosIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                var marcasExistentes = _context.Marcas
                    .Where(m => marcasIds.Contains(m.Id))
                    .Select(m => m.Id)
                    .ToListAsync();

                var categoriasExistentes = _context.Categorias
                    .Where(c => categoriasIds.Contains(c.Id))
                    .Select(c => c.Id)
                    .ToListAsync();

                var resultados = await Task.WhenAll(productosExistentes, marcasExistentes, categoriasExistentes);

                var idsProductosExistentes = resultados[0].ToHashSet();
                var idsMarcasExistentes = resultados[1].ToHashSet();
                var idsCategoriasExistentes = resultados[2].ToHashSet();

                // Filtrar productos válidos
                var productosValidos = productosArray
                    .Where(p => !idsProductosExistentes.Contains(p.Id) &&
                               idsMarcasExistentes.Contains(p.MarcaId) &&
                               idsCategoriasExistentes.Contains(p.CategoriaId))
                    .ToList();

                if (!productosValidos.Any())
                {
                    Console.WriteLine("[WARN] No hay productos válidos para insertar");
                    return;
                }

                // Inserción en lote
                await _context.Productos.AddRangeAsync(productosValidos);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[SUCCESS] {productosValidos.Count} productos insertados en lote");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error en inserción en lote: {ex.Message}");
                throw;
            }
        }

        // ===== MÉTODOS EXISTENTES (mantenidos para compatibilidad) =====

        public async Task DeleteAsync(Producto producto)
        {
            try
            {
                var preciosHistoricos = await _context.PreciosHistoricos.Where(ph => ph.ProductoId == producto.Id).ToListAsync();
                if (preciosHistoricos.Any())
                {
                    throw new InvalidOperationException("No se puede eliminar el producto porque tiene precios históricos asociados.");
                }
                _context.Remove(producto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    SqlException sqlException = dbEx.InnerException as SqlException;
                    if (sqlException.Number == 547) // Foreign key violation
                    {
                        throw new Exception("Error: El producto no puede ser eliminado debido a una violación de clave foránea.");
                    }
                }
                throw new Exception("Error al eliminar el producto a la base de datos.", dbEx);
            }
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            try
            {
                return await _context.Productos.Include(p => p.Marca).Include(p => p.Categoria).ToListAsync();
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de productos");
            }
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("Error buscando el producto: El ID del producto debe ser mayor que cero.");
                var producto = await _context.Productos.Include(p => p.Marca).Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
                if (producto == null)
                {
                    throw new KeyNotFoundException("El producto con el ID especificado no existe.");
                }
                return producto;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de productos");
            }
        }

        public async Task<Producto> GetByNombreAsync(string nombreProducto)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(nombreProducto))
                {
                    throw new ArgumentNullException("El producto que desea buscar no es válido");
                }
                Producto prodBuscado = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == nombreProducto);
                if (prodBuscado == null) throw new KeyNotFoundException("El producto con el nombre especificado no existe.");

                return prodBuscado;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de productos");
            }
        }

        public async Task UpdateAsync(Producto entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("El producto que desea actualizar no puede estar vacío");
            }
            if (entity.Id <= 0)
            {
                throw new ArgumentException("El ID del producto que desea actualizar no es válido");
            }
            try
            {
                var productoBuscado = await _context.Productos.FindAsync(entity.Id);
                if (productoBuscado == null)
                {
                    throw new KeyNotFoundException("El producto con el ID especificado no existe.");
                }
                productoBuscado.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 547) // Clave foránea
                    {
                        throw new InvalidOperationException("La actualización viola una restricción de clave foránea.", dbEx);
                    }
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627) // UNIQUE constraint violation
                    {
                        throw new InvalidOperationException("La actualización viola una restricción de unicidad.", dbEx);
                    }
                }
                throw new Exception("Error al actualizar el producto en la base de datos.", dbEx);
            }
        }

        public async Task<IEnumerable<Producto>> GetProductosByMarca(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("Marca no puede ser nula");
            try
            {
                return await _context.Productos
                    .Where(p => p.MarcaId == marca.Id)
                    .ToListAsync();
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de productos por marca");
            }
        }

        public async Task<Producto> GetProductoTodayWPrecioHistorico(int id)
        {
            try
            {
                DateOnly fechaHoy = TimeHelper.DateOnlyNowInMontevideo();

                var producto = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.Id == id && p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                        .ThenInclude(ph => ph.Supermercado)
                    .SingleOrDefaultAsync();

                return producto;
            }
            catch (DbException dbEx)
            {
                throw new Exception("Error al consultar la base de datos de precios");
            }
        }

        public async Task<PagedResult<Producto>> GetProductosTodayWPrecioHistorico(int pagina, int pageSize = 10)
        {
            try
            {
                DateOnly fechaHoy = TimeHelper.DateOnlyNowInMontevideo();

                var totalRecords = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .CountAsync();
                if (totalRecords == 0) throw new Exception("No existen precios para el día de hoy");
                if (pagina < 1 || pagina > (int)Math.Ceiling((double)totalRecords / pageSize))
                    throw new ArgumentOutOfRangeException("La página solicitada no es válida");

                var productos = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                        .ThenInclude(ph => ph.Supermercado)
                    .OrderBy(p => p.Id)
                    .Skip((pagina - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<Producto>
                {
                    Items = productos,
                    PaginaActual = pagina,
                    TotalPaginas = (int)Math.Ceiling((double)totalRecords / pageSize)
                };
            }
            catch (DbException)
            {
                throw new Exception("Error al consultar la base de datos de precios");
            }
        }

        public async Task<PagedResult<Producto>> GetProductosByNombreTodayWPrecioHistorico(string nombre, int pagina, int pageSize = 10)
        {
            try
            {
                DateOnly fechaHoy = TimeHelper.DateOnlyNowInMontevideo();

                var totalRecords = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.Nombre.Contains(nombre) && p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .CountAsync();
                if (totalRecords == 0) throw new Exception("No existen precios para el día de hoy");
                if (pagina < 1 || pagina > (int)Math.Ceiling((double)totalRecords / pageSize))
                    throw new ArgumentOutOfRangeException("La página solicitada no es válida");

                var productos = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.Nombre.Contains(nombre) && p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                        .ThenInclude(ph => ph.Supermercado)
                    .OrderBy(p => p.Id)
                    .Skip((pagina - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<Producto>
                {
                    Items = productos,
                    PaginaActual = pagina,
                    TotalPaginas = (int)Math.Ceiling((double)totalRecords / pageSize)
                };
            }
            catch (DbException)
            {
                throw new Exception("Error al consultar la base de datos de precios");
            }
        }

        public async Task<PagedResult<Producto>> GetByCategoriasWithPrecioHistoricoAsync(IEnumerable<int> categoriaIds, int pagina, int pageSize = 10)
        {
            DateOnly fechaHoy = TimeHelper.DateOnlyNowInMontevideo();

            var query = _context.Productos
                .AsNoTracking()
                .Where(p => categoriaIds.Contains(p.CategoriaId))
                .Where(p => p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                    .ThenInclude(ph => ph.Supermercado)
                .OrderBy(p => p.Id);

            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)pageSize);

            var productos = await query
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Producto>()
            {
                Items = productos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };
        }
    }
}