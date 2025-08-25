using SuperPrecios.Application.IRepository;
using SuperPrecios.Domain.Entities;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IProveedorRepository : IRepository<Proveedor>
    {
        public Task<Proveedor> GetByEmailAsync(string stringEmail);
        
    }
}
