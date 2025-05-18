using DataAccess.EntityFramework;
using LogicaAplicacion.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaAutenticacion.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EF
{
    public class IRepositoryMiembroEF : IRepositoryMiembro
    {
        private SuperPreciosContext _db;
        public IRepositoryMiembroEF(SuperPreciosContext db)
        {
            _db = db;            
        }
        void IRepository<Miembro>.Add(Miembro entity)
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
                if (dbEx != null)
                {
                    SqlException exSql = dbEx.InnerException as SqlException;
                    if(exSql != null)
                    {
                        if (exSql.Number == 2627) // Error de clave duplicada
                        {
                            throw new Exception("Error: El miembro ya existe en la base de datos", exSql);
                        }
                        else
                        {
                            throw new Exception("Error al agregar el miembro", exSql);
                        }
                    }
                    throw;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar el miembro {ex.Message}");
            }
        }

        void IRepository<Miembro>.Delete(Miembro entity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Miembro> IRepository<Miembro>.GetAll()
        {
            throw new NotImplementedException();
        }

        Miembro IRepository<Miembro>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        void IRepository<Miembro>.Update(Miembro entity)
        {
            throw new NotImplementedException();
        }
    }
}
