using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Proveedor;
using SuperPrecios.Domain.Exceptions;

namespace SuperPrecios.Application.Services.Proveedor
{
    public class ProveedorGetService : IProveedorGetService
    {
        private readonly IProveedorRepository _repo;

        public ProveedorGetService(IProveedorRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DtoProveedorGet>> Run()
        {
            var lista = await _repo.GetAllAsync();
            return MapperProveedor.ToDtoLista(lista);
        }

        public async Task<DtoProveedorGet> RunById(int id)
        {
            if (id < 1) throw new ProveedorException("El id no puede ser nulo");
            var proveedor = await _repo.GetByIdAsync(id);
            if (proveedor == null) throw new ProveedorException("No existe proveedor con ese id");
            return MapperProveedor.ToDto(proveedor);
        }

        public async Task<DtoProveedorUpdate> RunByIdUpdate(int id)
        {
            if (id < 1) throw new ProveedorException("El id no puede ser nulo");
            var proveedor = await _repo.GetByIdAsync(id);
            if (proveedor == null) throw new ProveedorException("No existe proveedor con ese id");
            return MapperProveedor.ToDtoUpdate(proveedor);
        }
    }
}
