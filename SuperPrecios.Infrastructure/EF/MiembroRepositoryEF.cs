using SuperPrecios.Infrastructure.EF;
using SuperPrecios.Application.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.AuthenticationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using SuperPrecios.AuthenticationCore.ValueObject;

namespace SuperPrecios.Infrastructure.EF
{
    public class MiembroRepositoryEF : IMiembroRepository
    {
        private SuperPreciosDbContext _context;
        public MiembroRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Miembro entity)
        {
            try
            {
                await _context.Miembros.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    SqlException exSql = dbEx.InnerException as SqlException;
                    if (exSql.Number == 2627 || exSql.Number == 2601)
                    {
                        throw new Exception("El miembro ya existe en la base de datos");
                    }
                }
                throw new Exception("Error al agregar el miembro a la base de datos");                
            }
        }

        public async Task DeleteAsync(Miembro miembro)
        {
            try
            {                                
                _context.Miembros.Remove(miembro);
                await _context.SaveChangesAsync();  
            }
            catch(DbUpdateException dbEx)
            {
                SqlException exSql = dbEx.InnerException as SqlException;
                if (exSql?.Number == 547) // Violación de restricción de clave foránea
                {
                    throw new Exception("BD Error: No se puede eliminar el miembro porque tiene registros relacionados");
                }
                throw new Exception($"BD Error: Error al eliminar el miembro de la base de datos {dbEx.Message}");                
            }
        }

        public async Task<IEnumerable<Miembro>> GetAllAsync()
        {
            try
            {                
                return await _context.Miembros.ToListAsync();
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }

        }

        public async Task<Miembro> GetByEmailAsync(string stringEmail)
        {
            if(string.IsNullOrWhiteSpace(stringEmail))
            {
                throw new ArgumentException("El email no puede ser nulo");
            }
            try
            {
                Email email = new Email(stringEmail);                
                return await _context.Miembros.FirstOrDefaultAsync(m => m.Email == email);
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }            
        }

        public async Task<IEnumerable<Miembro>> GetByEmailListAsync(string stringEmail)
        {
            if (string.IsNullOrWhiteSpace(stringEmail)) throw new ArgumentException("El email no puede ser nulo");            
            try
            {
                Email email = new Email(stringEmail);                
                return await _context.Miembros.Where(m => m.Email == email).ToListAsync();
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Miembro> GetByIdAsync(int id)
        {
            try
            {
                Miembro miembro = await _context.Miembros.FindAsync(id);
                return miembro;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task UpdateAsync(Miembro miembroActualizado)
        {
            try
            {                                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"BD Error: Error al actualizar el miembro en la base de datos. {dbEx.Message}", dbEx);
            }
        }        
    }
}
