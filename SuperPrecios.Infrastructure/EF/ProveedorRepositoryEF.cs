using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using SuperPrecios.AuthenticationCore.Entities;
using SuperPrecios.AuthenticationCore.ValueObject;

namespace SuperPrecios.Infrastructure.EF
{
    public class ProveedorRepositoryEF : IProveedorRepository
    {
        private readonly SuperPreciosDbContext _context;

        public ProveedorRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Proveedor entity)
        {
            try
            {
                await _context.Proveedores.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException exSql &&
                    (exSql.Number == 2627 || exSql.Number == 2601))
                {
                    throw new Exception("El proveedor ya existe en la base de datos");
                }
                throw new Exception("Error al agregar el proveedor a la base de datos");
            }
        }

        public async Task DeleteAsync(Proveedor entity)
        {
            try
            {
                _context.Proveedores.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException exSql && exSql.Number == 547)
                {
                    throw new Exception("BD Error: No se puede eliminar el proveedor porque tiene registros relacionados");
                }
                throw new Exception($"BD Error: Error al eliminar el proveedor de la base de datos {dbEx.Message}");
            }
        }

        public async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            try
            {
                return await _context.Proveedores
                    .Include(p => p.Supermercado)
                    .ToListAsync();
            }
            catch (DbException)
            {
                throw new Exception("BD Error: al consultar la base de datos de proveedores");
            }
        }

        public async Task<Proveedor> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Proveedores
                    .Include(p => p.Supermercado)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (DbException)
            {
                throw new Exception("BD Error: al consultar la base de datos de proveedores");
            }
        }

        public async Task<Proveedor> GetByEmailAsync(string stringEmail)
        {
            if (string.IsNullOrWhiteSpace(stringEmail))
            {
                throw new ArgumentException("El email no puede ser nulo");
            }
            try
            {
                Email email = new Email(stringEmail);
                return await _context.Proveedores.FirstOrDefaultAsync(m => m.Email == email);
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }

        public async Task UpdateAsync(Proveedor entity)
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"BD Error: Error al actualizar el proveedor en la base de datos. {dbEx.Message}", dbEx);
            }
        }


    }
}
