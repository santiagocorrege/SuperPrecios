using SuperPrecios.Domain.Entities;

namespace SuperPrecios.Domain.IRepositories
{
    public interface ICarritoRepository
    {        
        Task<Carrito?> GetByIdAsync(int id);
        Task<Carrito?> GetByUsuarioIdAsync(int usuarioId);        
        Task SaveAsync();
    }
}