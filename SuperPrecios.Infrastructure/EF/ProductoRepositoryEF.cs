using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entidades;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
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

        public async Task AddAsync(Producto entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("El producto no puede ser nulo");
            }
            try
            {
                if(entity.Marca != null)
                {
                    _context.Entry(entity.Marca).State = EntityState.Unchanged;
                }
                if (entity.Categoria != null)
                {
                    _context.Entry(entity.Categoria).State = EntityState.Unchanged;
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
            catch (Exception ex)
            {
                throw new Exception("Error al agregar el producto a la base de datos.", ex);
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
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el producto a la base de datos.", ex);
            }
        }

        public async Task<IEnumerable<Producto>> GetAll()
        {
            try
            {
                return await _context.Productos.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos de la base de datos.", ex);
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
            catch(Exception ex)
            {
                throw new Exception("Error al buscar el producto en la base de datos.", ex);
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
            throw new Exception("Error al actualizar el producto en la base de datos.");
        }
    }
}
