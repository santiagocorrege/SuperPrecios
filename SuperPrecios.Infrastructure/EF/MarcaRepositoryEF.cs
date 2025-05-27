using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entidades;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class MarcaRepositoryEF : IMarcaRepository
    {
        private readonly SuperPreciosDbContext _context;

        public MarcaRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Marca entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "La marca no puede ser nula");

            try
            {                
                await _context.Marcas.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 2627) // Unique constraint
                        throw new Exception("Error: La marca ya existe en la base de datos.");
                    if (sqlException.Number == 547) // Foreign key violation (poco probable aquí)
                        throw new Exception("Error: Violación de clave foránea al agregar la marca.");
                }
                throw new Exception("Error al agregar la marca a la base de datos.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar la marca a la base de datos.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la marca debe ser mayor que cero.", nameof(id));

            try
            {
                var marca = await _context.Marcas.FindAsync(id);
                if (marca == null)
                    throw new KeyNotFoundException("La marca con el ID especificado no existe.");

                _context.Marcas.Remove(marca);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 547) // Foreign key violation (ej. productos relacionados)
                        throw new Exception("Error: La marca no puede ser eliminada debido a que tiene productos asociados.");
                }
                throw new Exception("Error al eliminar la marca de la base de datos.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la marca de la base de datos.", ex);
            }
        }

        public async Task<IEnumerable<Marca>> GetAll()
        {
            try
            {
                return await _context.Marcas.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las marcas de la base de datos.", ex);
            }
        }

        public async Task<Marca> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la marca debe ser mayor que cero.", nameof(id));

            try
            {
                var marca = await _context.Marcas.FindAsync(id);
                if (marca == null)
                    throw new KeyNotFoundException("La marca con el ID especificado no existe.");

                return marca;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar la marca en la base de datos.", ex);
            }
        }

        public async Task UpdateAsync(Marca entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "La marca no puede ser nula");
            if (entity.Id <= 0)
                throw new ArgumentException("El ID de la marca debe ser mayor que cero.", nameof(entity.Id));

            try
            {
                var marcaBuscada = await _context.Marcas.FindAsync(entity.Id);
                if (marcaBuscada == null)
                    throw new KeyNotFoundException("La marca con el ID especificado no existe.");

                // Actualizamos solo las propiedades relevantes
                marcaBuscada.Nombre = entity.Nombre;
                marcaBuscada.Validate();

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 547) // Foreign key violation (improbable aquí)
                        throw new InvalidOperationException("La actualización viola una restricción de clave foránea.", dbEx);
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627) // Unique constraint
                        throw new InvalidOperationException("La actualización viola una restricción de unicidad.", dbEx);
                }
                throw new Exception("Error al actualizar la marca en la base de datos.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la marca en la base de datos.", ex);
            }
        }
    }
}
