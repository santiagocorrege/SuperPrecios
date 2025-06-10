using Microsoft.Data.SqlClient;
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
    public class SupermercadoRepositoryEF : ISupermercadoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public SupermercadoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Supermercado entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "El supermercado no puede ser nulo");

            try
            {
                // Validar entidad antes de agregar
                entity.Validate();

                await _context.Supermercados.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    // 2627: Unique constraint violation (por ejemplo, si existe un índice UNIQUE sobre Nombre)
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        throw new Exception("Error: Ya existe un supermercado con el mismo nombre.");

                    // 547: Foreign key violation (rara en AddAsync de supermercado, pero se incluye por consistencia)
                    if (sqlEx.Number == 547)
                        throw new Exception("Error: Violación de clave foránea al agregar el supermercado.");
                }

                throw new Exception("Error al agregar el supermercado a la base de datos.", dbEx);
            }            
        }

        public async Task DeleteAsync(Supermercado supermercado)
        {
            try
            {                
                var proveedor = await _context.Proveedores.AnyAsync(p => p.SupermercadoId == supermercado.Id);
                if (proveedor)
                    throw new Exception("No se puede eliminar el supermercado porque tiene un proveedor asociado.");
                var preciosHistoricos = await _context.PreciosHistoricos.AnyAsync(ph => ph.SupermercadoId == supermercado.Id);
                if (preciosHistoricos) throw new Exception("No se puede eliminar el supermercado porque tiene precios históricos asociados.");
                _context.Supermercados.Remove(supermercado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    // 547: Foreign key violation (por ejemplo, si hay PreciosHistoricos apuntando a este supermercado)
                    if (sqlEx.Number == 547)
                        throw new Exception("Error: El supermercado no puede eliminarse porque tiene datos relacionados en la base de datos.");
                }

                throw new Exception("Error al eliminar el supermercado de la base de datos.", dbEx);
            }
        }

        public async Task<IEnumerable<Supermercado>> GetAllAsync()
        {
            try
            {
                return await _context.Supermercados
                                     .AsNoTracking()
                                     .ToListAsync();
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Supermercado> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del supermercado debe ser mayor que cero.", nameof(id));

            try
            {
                var supermercado = await _context.Supermercados.FindAsync(id);
                if (supermercado == null)
                    throw new KeyNotFoundException("El supermercado con el ID especificado no existe.");

                return supermercado;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Supermercado> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del supermercado no puede estar vacío.", nameof(name));

            try
            {
                var formattedName = UtilidadesString.FormatearTexto(name);
                return await _context.Supermercados
                .Include(s => s.Proveedor)                        
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Nombre == formattedName);
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task UpdateAsync(Supermercado entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "El supermercado no puede ser nulo");
            if (entity.Id <= 0)
                throw new ArgumentException("El ID del supermercado debe ser mayor que cero.", nameof(entity.Id));

            try
            {
                var supermercadoExistente = await _context.Supermercados.FindAsync(entity.Id);
                if (supermercadoExistente == null)
                    throw new KeyNotFoundException("El supermercado con el ID especificado no existe.");

                // Actualizar campos
                supermercadoExistente.Nombre = UtilidadesString.FormatearTexto(entity.Nombre);
                supermercadoExistente.WebsiteUrl = entity.WebsiteUrl;
                supermercadoExistente.Validate();

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    // 547: Foreign key violation (aunque es raro en una actualización de Supermercado)
                    if (sqlEx.Number == 547)
                        throw new Exception("Error: La actualización viola una restricción de clave foránea.");

                    // 2601 / 2627: Unique constraint violation
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                        throw new Exception("Error: Ya existe otro supermercado con el mismo nombre.");
                }

                throw new Exception("Error al actualizar el supermercado en la base de datos.", dbEx);
            }
        }
    }
}
