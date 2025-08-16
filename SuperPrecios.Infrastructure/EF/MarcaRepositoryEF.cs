using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public async Task AddAsync(Marca marca)
        {
            if (marca == null)
                throw new ArgumentNullException("La marca no puede ser nula");

            try
            {
                // ✅ VALIDACIÓN: ID debe ser proporcionado por Python
                if (marca.Id <= 0)
                    throw new ArgumentException("El ID de la marca debe ser proporcionado por el sistema externo");

                await _context.Marcas.AddAsync(marca);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 2627) // Unique constraint
                        throw new Exception($"Error: La marca con ID {marca.Id} ya existe en la base de datos.");
                    if (sqlException.Number == 547) // Foreign key violation
                        throw new Exception("Error: Violación de clave foránea al agregar la marca.");
                }
                throw new Exception("Error al agregar la marca a la base de datos.", dbEx);
            }
        }

        // ✅ NUEVO MÉTODO: Inserción en lote optimizada para Matching
        public async Task AddRangeAsync(IEnumerable<Marca> marcas)
        {
            if (marcas == null || !marcas.Any())
                return;

            var marcasArray = marcas.ToArray();
            Console.WriteLine($"[INFO] Insertando {marcasArray.Length} marcas en lote...");

            try
            {
                // ✅ OPTIMIZACIÓN: Configurar timeout para operaciones grandes
                _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

                // ✅ OPTIMIZACIÓN: Deshabilitar AutoDetectChanges para mejor performance
                var originalAutoDetect = _context.ChangeTracker.AutoDetectChangesEnabled;
                _context.ChangeTracker.AutoDetectChangesEnabled = false;

                try
                {
                    // ✅ INSERCIÓN EN LOTE: Una sola operación
                    await _context.Marcas.AddRangeAsync(marcasArray);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"[SUCCESS] {marcasArray.Length} marcas insertadas en lote");
                }
                finally
                {
                    // ✅ RESTAURAR: AutoDetectChanges
                    _context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetect;
                }
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 2627) // Unique constraint
                        throw new Exception("Error: Marcas duplicadas detectadas en el lote.");
                    if (sqlException.Number == 547) // Foreign key violation
                        throw new Exception("Error: Violación de clave foránea al agregar marcas en lote.");
                }
                throw new Exception("Error al agregar marcas en lote a la base de datos.", dbEx);
            }
        }

        public async Task DeleteAsync(Marca marca)
        {
            try
            {
                _context.Marcas.Remove(marca);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 547) // Foreign key violation (productos relacionados)
                        throw new Exception("La marca no puede ser eliminada debido a que tiene productos asociados.");
                }
                throw new Exception("Error al eliminar la marca de la base de datos.", dbEx);
            }
        }

        public async Task<IEnumerable<Marca>> GetAllAsync()
        {
            try
            {
                return await _context.Marcas
                    .AsNoTracking() // ✅ CORREGIDO: AsNoTracking antes del ToListAsync
                    .ToListAsync();
            }
            catch (DbException)
            {
                throw new Exception("Error al obtener las marcas de la base de datos.");
            }
        }

        public async Task<Marca> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la marca debe ser mayor que cero.", nameof(id));

            try
            {
                var marca = await _context.Marcas
                    .AsNoTracking() // ✅ CORREGIDO
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (marca == null)
                    throw new KeyNotFoundException("La marca con el ID especificado no existe.");

                return marca;
            }
            catch (DbException)
            {
                throw new Exception("Error al buscar la marca en la base de datos.");
            }
        }

        public async Task<Marca> GetByNombreAsync(Marca marca)
        {
            if (String.IsNullOrWhiteSpace(marca.Nombre))
            {
                throw new ArgumentException("El nombre de la marca no puede ser nulo o vacío.");
            }

            try
            {
                var marcaBuscada = await _context.Marcas
                    .AsNoTracking() // ✅ CORREGIDO
                    .FirstOrDefaultAsync(m => m.Nombre.Equals(marca.Nombre));

                return marcaBuscada;
            }
            catch (DbException)
            {
                throw new Exception("Error al buscar la marca por nombre en la base de datos.");
            }
        }

        // ✅ NUEVO MÉTODO: Obtener múltiples marcas por IDs (optimización para Matching)
        public async Task<IEnumerable<Marca>> GetMarcasByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Marca>();

            try
            {
                var idsArray = ids.ToArray();
                return await _context.Marcas
                    .AsNoTracking()
                    .Where(m => idsArray.Contains(m.Id))
                    .ToListAsync();
            }
            catch (DbException)
            {
                throw new Exception("Error al obtener las marcas por IDs.");
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
                    if (sqlEx.Number == 547) // Foreign key violation
                        throw new InvalidOperationException("La actualización viola una restricción de clave foránea.", dbEx);
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627) // Unique constraint
                        throw new InvalidOperationException("La actualización viola una restricción de unicidad.", dbEx);
                }
                throw new Exception("Error al actualizar la marca en la base de datos.", dbEx);
            }
        }
    }
}