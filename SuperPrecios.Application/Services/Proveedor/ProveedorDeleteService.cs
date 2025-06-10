using SuperPrecios.Application.IServices.Proveedor;
using SuperPrecios.Domain.IRepositories;

namespace SuperPrecios.Application.Services.Proveedor
{
    public class ProveedorDeleteService : IProveedorDeleteService
    {
        private readonly IProveedorRepository _proveedorRepository;

        public ProveedorDeleteService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        public async Task Run(int id)
        {
            if (id < 1) throw new ArgumentNullException("El id no puede ser menor a 0");
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null) throw new KeyNotFoundException("El proveedor no existe");            
            await _proveedorRepository.DeleteAsync(proveedor);
        }
    }
}
