using SuperPrecios.Infrastructure.EF;
using SuperPrecios.Application.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.AutenticacionCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class MiembroRepositoryEF : IMiembroRepository
    {
        private SuperPreciosContext _db;
        public MiembroRepositoryEF(SuperPreciosContext db)
        {
            _db = db;            
        }
        public async Task AddAsync(Miembro entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            try
            {
                await _db.Miembros.AddAsync(entity);
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateException dbEx)
            {
                SqlException exSql = dbEx.InnerException as SqlException;
                if(exSql?.Number != null)
                {
                    if (exSql.Number == 2627) // Error de clave duplicada
                    {
                        throw new Exception($"BD Error: El miembro ya existe en la base de datos", exSql);
                    }
                    else
                    {
                        throw new Exception($"BD Error: al agregar el miembro | Mensaje {exSql.Message}", exSql);
                    }
                }
                throw;                                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar el miembro | Mensaje {ex.Message}", ex);
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
                Miembro miembroBuscado = await _db.Miembros.FindAsync(id);
                if(miembroBuscado == null)
                {
                    throw new ArgumentNullException("El miembro no existe en la base de datos");
                }
                _db.Miembros.Remove(miembroBuscado);
                await _db.SaveChangesAsync();  
            }
            catch(DbUpdateException dbEx)
            {
                SqlException exSql = dbEx.InnerException as SqlException;
                if (exSql?.Number == 547) // Violación de restricción de clave foránea
                {
                    throw new Exception("BD Error: No se puede eliminar el miembro porque tiene registros relacionados", exSql);
                }
                else
                {
                    throw new Exception($"BD Error: Error al eliminar el miembro de la base de datos {dbEx.Message}", dbEx);
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Error al eliminar el miembro {ex.Message}");
            }
        }

        public async Task<IEnumerable<Miembro>> GetAll()
        {
            try
            {                
                return await _db.Miembros.ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los miembros {ex.Message}");
            }

        }

        public async Task<Miembro> GetByEmailAsync(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("El email no puede ser nulo");
            }
            Miembro miembroBuscado = await _db.Miembros.FirstOrDefaultAsync(m => m.Email.Valor == email);            
            if(miembroBuscado == null)
            {
                throw new KeyNotFoundException("El miembro con ese email no existe en la base de datos");
            }
            return miembroBuscado;
        }

        public async Task<Miembro> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("DB Error: El id no puede ser nulo");
            }
            try
            {
                Miembro miembro = await _db.Miembros.FindAsync(id);
                if (miembro == null)
                {
                    throw new Exception("El miembro no existe en la base de datos");
                }
                return miembro;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"BD Error: al consultar la base de datos de miembros {dbEx.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el miembro {ex.Message}");
            }
        }

        public async Task UpdateAsync(Miembro entity)
        {
            if(entity == null || entity.Id <= 0)
            {
                throw new ArgumentNullException("Error: El miembro no puede ser nula");
            }
            try
            {
                Miembro miembro = await _db.Miembros.FindAsync(entity.Id);
                if (miembro == null)
                {
                    throw new KeyNotFoundException($"Error: No se encontró un miembro con ID {entity.Id}");
                }
                // Si Modificar modifica propiedades de la entidad recuperada(miembro), podés omitir Update:
                miembro.Modificar(entity);
                _db.Miembros.Update(miembro);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"BD Error: Error al actualizar el miembro en la base de datos. {dbEx.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el miembro: {ex.Message}", ex);
            }
        }        
    }
}
