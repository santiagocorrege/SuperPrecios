using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    throw new ArgumentException("El ID de la marca debe ser mayor que cero o la marca no puede ser nula.", nameof(entity.MarcaId));
                }
                if(entity.CategoriaId <= 0)
                {
                    throw new ArgumentException("El ID de la categoria debe ser mayor que cero o la categoria no puede ser nula.", nameof(entity.CategoriaId));
                }
                Marca marcaBuscada = await _context.Marcas.FindAsync(entity.MarcaId);
                if (marcaBuscada != null)
                {
                    entity.Marca = marcaBuscada;
                    _context.Entry(entity.Marca).State = EntityState.Unchanged;
                }
                else
                {
                    throw new ArgumentException("El ID de la marca especificada no existe.", nameof(entity.MarcaId));
                }
                Categoria categoriaBuscada = await _context.Categorias.FindAsync(entity.CategoriaId);
                if (categoriaBuscada != null)
                {
                    entity.Categoria = categoriaBuscada;
                    _context.Entry(entity.Categoria).State = EntityState.Unchanged;
                }
                else
                {
                    throw new ArgumentException("El ID de la categoria especificada no existe.", nameof(entity.CategoriaId));
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

        public async Task AddAsync(Producto producto)
        {
            if (producto == null || producto.CategoriaId < 1 || producto.MarcaId < 1)
            {
                throw new ArgumentNullException("Error: Ingrese todos los campos por favor");
            }
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

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del producto debe ser mayor que cero.", nameof(id));
            }
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    throw new KeyNotFoundException("El producto con el ID especificado no existe.");
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

        public async Task<IEnumerable<Producto>> GetAll()
        {
            try
            {
                return await _context.Productos.ToListAsync();
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentException("Error buscando el producto: El ID del producto debe ser mayor que cero.");
            }
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if(producto == null)
                {
                    throw new Exception("Error buscando el producto: El producto con el ID especificado no existe.");
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
                if(nombreProducto == null)
                {
                    throw new ArgumentNullException("El producto que desea agregar no es valido");
                }
                Producto prodBuscado = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == nombreProducto);
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
                throw new ArgumentNullException("El producto no puede ser nulo");
            }
            if(entity.Id <= 0)
            {
                throw new ArgumentException("El ID del producto debe ser mayor que cero.", nameof(entity.Id));
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
    }
}
