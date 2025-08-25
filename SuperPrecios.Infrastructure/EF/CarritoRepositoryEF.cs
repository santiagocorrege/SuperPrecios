using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System.Data.Common;

namespace SuperPrecios.Infrastructure.EF
{
    public class CarritoRepositoryEF : ICarritoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public CarritoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }

        public async Task<Carrito?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Carritos
                    .AsNoTracking()
                    .Include(c => c.Lineas)
                        .ThenInclude(l => l.Producto)                                                
                    .FirstOrDefaultAsync(c => c.Id == id);
                /*
                 .Include(c => c.Lineas)
                        .ThenInclude(l => l.Producto)
                            .ThenInclude(p => p.Marca)
                    .Include(c => c.Usuario)
                    .FirstOrDefaultAsync(c => c.Id == id);
                 */
            }
            catch (DbException ex)
            {
                throw new Exception($"Error al obtener el carrito con ID {id} de la base de datos", ex);
            }
        }

        public async Task<Carrito?> GetByUsuarioIdAsync(int usuarioId)
        {
            try
            {
                return await _context.Carritos                    
                    .Include(c => c.Lineas)
                        .ThenInclude(l => l.Producto)                                                
                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

                /*
                    .Include(c => c.Lineas)
                        .ThenInclude(l => l.Producto)
                            .ThenInclude(p => p.Marca)
                    .Include(c => c.Usuario)
                    .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
                 */
            }
            catch (DbException ex)
            {
                throw new Exception($"Error al obtener el carrito del usuario {usuarioId} de la base de datos", ex);
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error al guardar los cambios del carrito en la base de datos", ex);
            }
            catch (DbException ex)
            {
                throw new Exception("Error de conexión con la base de datos", ex);
            }
        }
    }
}