using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class CategoriaRepositoryEF : ICategoriaRepository
    {
        private readonly SuperPreciosDbContext _context;
        public CategoriaRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Categoria entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "La categoría no puede ser nula");

            try
            {                
                await _context.Categorias.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 2627) // Unique constraint violation (Producto único)
                        throw new CategoriaException("Error: La categoría ya existe en la base de datos.");
                    if (sqlException.Number == 547) // Foreign key violation
                        throw new CategoriaException("Error: Violación de clave foránea al agregar la categoría.");
                }
                throw new Exception("Error al agregar la categoría a la base de datos.", dbEx);
            }
        }

        public async Task DeleteAsync(Categoria categoria)
        {
            try
            {
                bool existeProducto = await _context.Productos.AnyAsync(p => p.CategoriaId == categoria.Id);
                if(existeProducto) throw new CategoriaException("La categoría no puede ser eliminada porque tiene productos asociados.");
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {                
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 547) // Foreign key violation (productos asociados)
                        throw new CategoriaException("La categoría no puede ser eliminada porque tiene productos asociados.");
                }
                throw new Exception($"Error al eliminar la categoría de la base de datos.");
            }
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            try
            {
                return await _context.Categorias.ToListAsync();
            }
            catch(DbException dbEx)
            {
                throw new Exception("Error al obtener las categorías de la base de datos.");
            }             
        }

        public async Task<Categoria> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la categoría debe ser mayor que cero.", nameof(id));

            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                    throw new KeyNotFoundException("La categoría con el ID especificado no existe.");

                return categoria;
            }
            catch (DbException ex)
            {
                throw new Exception("Error al buscar la categoría en la base de datos.");
            }
        }

        public async Task<Categoria> GetByNombreAsync(Categoria categoria)
        {            
            if (categoria == null)
            {
                throw new ArgumentNullException("La categoria no puede estar vacia");
            }
            try
            {
                return  await _context.Categorias.FirstOrDefaultAsync(c=> c.Nombre == categoria.Nombre);
            }
            catch (DbException ex)
            {
                throw new Exception("Error al buscar la categoría por nombre en la base de datos.");
            }
        }

        public async Task UpdateAsync(Categoria entity)
        {
            if (entity == null)
                throw new ArgumentNullException("La categoría no puede ser nula");
            if (entity.Id <= 0)
                throw new ArgumentException("El ID de la categoría debe ser mayor que cero.");

            try
            {
                var categoriaBuscada = await _context.Categorias.FindAsync(entity.Id);
                if (categoriaBuscada == null)
                    throw new KeyNotFoundException("La categoría con el ID especificado no existe.");

                categoriaBuscada.Nombre = entity.Nombre;
                categoriaBuscada.Validate();

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 547) // Foreign key violation
                        throw new InvalidOperationException("La actualización viola una restricción de clave foránea.", dbEx);
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627) // Unique constraint
                        throw new InvalidOperationException("La actualización viola una restricción de unicidad.", dbEx);
                }
                throw new Exception("Error al actualizar la categoría en la base de datos.", dbEx);
            }

        }
    }
}
