using SuperPrecios.Infrastructure.EntityFramework;
using SuperPrecios.Application.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.AutenticacionCore.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class RepositoryMiembroEF : IRepositoryMiembro
    {
        private SuperPreciosContext _db;
        public RepositoryMiembroEF(SuperPreciosContext db)
        {
            _db = db;            
        }
        public void Add(Miembro entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            try
            {
                _db.Add(entity);
                _db.SaveChanges();
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

        public void Delete(Miembro entity)
        {
            if(entity == null || entity.Id <= 0)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            try
            {                
                Miembro miembroBuscado = _db.Miembros.Find(entity.Id);
                if(miembroBuscado == null)
                {
                    throw new Exception("El miembro no existe en la base de datos");
                }
                _db.Miembros.Remove(miembroBuscado);
                _db.SaveChanges();  
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

        public IEnumerable<Miembro> GetAll()
        {
            try
            {                
                return _db.Miembros.ToList();
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

        public Miembro GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException("DB Error: El id no puede ser nulo");
            }
            try
            {
                Miembro miembro = _db.Miembros.Find(id);
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

        public void Update(Miembro entity)
        {
            if(entity == null || entity.Id <= 0)
            {
                throw new Exception("Error: El miembrto no puede ser nula");
            }
            try
            {
                Miembro miembro = _db.Miembros.Find(entity.Id);
                if (miembro == null)
                {
                    throw new KeyNotFoundException($"Error: No se encontró un miembro con ID {entity.Id}");
                }
                miembro.Modificar(entity);
                _db.Miembros.Update(miembro);
                _db.SaveChanges();
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
