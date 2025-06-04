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
            if (entity == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
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

        public async Task DeleteAsync(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentException("El id del miembro no puede ser nulo");
            }
            try
            {                
                Miembro miembroBuscado = await _context.Miembros.FindAsync(id);
                if(miembroBuscado == null)
                {
                    throw new ArgumentNullException("El miembro no existe en la base de datos");
                }
                _context.Miembros.Remove(miembroBuscado);
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

        public async Task<IEnumerable<Miembro>> GetAll()
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
                Miembro miembroBuscado = await _context.Miembros.FirstOrDefaultAsync(m => m.Email == email);
                if (miembroBuscado == null)
                {
                    throw new KeyNotFoundException("El miembro con ese email no existe en la base de datos");
                }
                return miembroBuscado;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }            
        }

        public async Task<IEnumerable<Miembro>> GetByEmailListAsync(string stringEmail)
        {
            if (string.IsNullOrWhiteSpace(stringEmail))
            {
                throw new ArgumentException("El email no puede ser nulo");
            }
            try
            {
                Email email = new Email(stringEmail);
                var miembrosBuscado = await _context.Miembros.Where(m => m.Email == email).ToListAsync();
                return miembrosBuscado;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task<Miembro> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("DB Error: El id no puede ser nulo");
            }
            try
            {
                Miembro miembro = await _context.Miembros.FindAsync(id);
                if (miembro == null)
                {
                    throw new Exception("El miembro no existe en la base de datos");
                }
                return miembro;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task UpdateAsync(Miembro miembroActualizado)
        {
            if(miembroActualizado == null || miembroActualizado.Id <= 0)
            {
                throw new ArgumentNullException("Error: El miembro no puede ser nula");
            }
            try
            {
                Miembro miembro = await _context.Miembros.FindAsync(miembroActualizado.Id);
                if (miembro == null)
                {
                    throw new KeyNotFoundException("No se encontró un miembro con ese Id");
                }
                miembro.Modificar(miembroActualizado);       
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"BD Error: Error al actualizar el miembro en la base de datos. {dbEx.Message}", dbEx);
            }
        }        
    }
}
