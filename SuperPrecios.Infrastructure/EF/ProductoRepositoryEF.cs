using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.Common;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
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

        public async Task AddAsyncCompleto(Producto entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("El producto no puede ser nulo");
            }
            try
            {
                if(entity.MarcaId <= 0)
                {
                    throw new ArgumentException("El ID de la marca debe ser mayor que cero o la marca no puede ser nula.");
                }
                if(entity.CategoriaId <= 0)
                {
                    throw new ArgumentException("El ID de la categoria debe ser mayor que cero o la categoria no puede ser nula.");
                }
                Marca marcaBuscada = await _context.Marcas.FindAsync(entity.MarcaId);
                if (marcaBuscada != null)
                {
                    entity.Marca = marcaBuscada;
                    _context.Entry(entity.Marca).State = EntityState.Unchanged;
                }
                else
                {
                    throw new ArgumentException("El ID de la marca especificada no existe.");
                }
                Categoria categoriaBuscada = await _context.Categorias.FindAsync(entity.CategoriaId);
                if (categoriaBuscada != null)
                {
                    entity.Categoria = categoriaBuscada;
                    _context.Entry(entity.Categoria).State = EntityState.Unchanged;
                }
                else
                {
                    throw new ArgumentException("El ID de la categoria especificada no existe.");
                }
                await _context.Productos.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    SqlException sqlException = dbEx.InnerException as SqlException;
                    if (sqlException.Number == 2627) // Unique constraint error
                    {
                        throw new Exception("Error: El producto ya existe en la base de datos.");
                    }
                    if (sqlException.Number == 547) // Foreign key violation
                    {
                        throw new Exception("Error: El producto no puede ser agregado debido a una violación de clave foránea.");
                    }
                }
                throw new Exception("Error al agregar el producto a la base de datos.", dbEx);
            }
        }
        //SE PUEDE ELIMINAR?
        public async Task AddAsync(Producto producto)
        {
            try
            {
                var categoriaBuscado = await _context.Categorias.FindAsync(producto.CategoriaId);
                if (categoriaBuscado == null)
                {
                    throw new ArgumentException("El la categoria especificada no existe.");
                }
                var marcaBuscado = await _context.Marcas.FindAsync(producto.MarcaId);
                if (marcaBuscado == null)
                {
                    throw new ArgumentException("El la marca especificada no existe.");
                }
                var productoBuscado = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == producto.Nombre && p.MarcaId == producto.MarcaId);
                if(productoBuscado != null)
                {
                    throw new ArgumentException("El producto ya existe en la base de datos.");
                }
                await _context.Productos.AddAsync(producto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    SqlException sqlException = dbEx.InnerException as SqlException;
                    if (sqlException.Number == 2627) // Unique constraint error
                    {
                        throw new Exception("Error: El producto ya existe en la base de datos.");
                    }
                    if (sqlException.Number == 547) // Foreign key violation
                    {
                        throw new Exception("Error: El producto no puede ser agregado debido a una violación de clave foránea.");
                    }
                }
                throw new Exception("Error al agregar el producto a la base de datos.", dbEx);
            }
        }

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
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Producto> GetByIdAsync(int id)
        {                        
            try
            {
                if (id <= 0) throw new ArgumentException("Error buscando el producto: El ID del producto debe ser mayor que cero.");
                var producto = await _context.Productos.Include(p => p.Marca).Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
                if(producto == null)
                {
                    throw new Exception("El producto con el ID especificado no existe.");
                }
                return producto;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Producto> GetByNombreAsync(string nombreProducto)
        {
            try
            {
                if(String.IsNullOrWhiteSpace(nombreProducto))
                {
                    throw new ArgumentNullException("El producto que desea agregar no es valido");
                }
                Producto prodBuscado = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == nombreProducto);
                if (prodBuscado == null) throw new KeyNotFoundException("El producto con el nombre especificado no existe.");

                return prodBuscado;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task UpdateAsync(Producto entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException("El producto que desea agregar no puede estar vacio");
            }
            if(entity.Id <= 0)
            {
                throw new ArgumentException("El ID del producto que desea agregar no es valido");
            }
            try
            {
                var productoBuscado = await _context.Productos.FindAsync(entity.Id);
                if(productoBuscado == null)
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
            }            
        }

        public async Task<IEnumerable<Producto>> GetProductosByMarca(Marca marca)
        {
            if(marca == null) throw new ArgumentNullException("Marca no puede ser nula");
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
                DateOnly fechaHoy = DateOnly.FromDateTime(DateTime.Now);

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
                DateOnly fechaHoy = DateOnly.FromDateTime(DateTime.Now);

                var totalRecords = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .CountAsync();
                if (totalRecords == 0) throw new Exception("No existen precios para el dia de hoy");
                if (pagina < 1 || pagina > (int)Math.Ceiling((double)totalRecords / pageSize))
                    throw new ArgumentOutOfRangeException("La pagina solicitada no es valida");

                var productos = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy)) // filtro
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                        .ThenInclude(ph => ph.Supermercado)
                    .OrderBy(p => p.Id)                            // <-- aquí el ORDER BY
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
                DateOnly fechaHoy = DateOnly.FromDateTime(DateTime.Now);

                var totalRecords = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.Nombre.Contains(nombre) && p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .CountAsync();
                if (totalRecords == 0) throw new Exception("No existen precios para el dia de hoy");
                if (pagina < 1 || pagina > (int)Math.Ceiling((double)totalRecords / pageSize))
                    throw new ArgumentOutOfRangeException("La pagina solicitada no es valida");

                var productos = await _context.Productos
                    .AsNoTracking()
                    .Where(p => p.Nombre.Contains(nombre) && p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                        .ThenInclude(ph => ph.Supermercado)
                    .OrderBy(p => p.Id)                        // <-- añadido aquí
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
            var fechaHoy = DateOnly.FromDateTime(DateTime.Now);

            // Prepara la consulta base
            var query = _context.Productos
                .AsNoTracking()
                .Where(p => categoriaIds.Contains(p.CategoriaId))
                .Where(p => p.PreciosHistoricos.Any(ph => ph.Fecha == fechaHoy))
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .Include(p => p.PreciosHistoricos.Where(ph => ph.Fecha == fechaHoy))
                    .ThenInclude(ph => ph.Supermercado)
                .OrderBy(p => p.Id);    // <-- Orden por Id antes del Skip/Take

            // Cálculo de totales
            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)pageSize);

            // Paginación
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
