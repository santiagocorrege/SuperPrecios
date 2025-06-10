using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Proveedor;
using ProveedorCore = SuperPrecios.Domain.Entities.Proveedor;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;

namespace SuperPrecios.Application.Services.Proveedor
{
    public class ProveedorAddService : IProveedorAddService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ISupermercadoRepository _supermercadoRepository;

        public ProveedorAddService(IProveedorRepository proveedorRepo, ISupermercadoRepository supermercadoRepository)
        {
            _proveedorRepository = proveedorRepo;
            _supermercadoRepository = supermercadoRepository;

        }

        public async Task Run(DtoProveedorAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("El proveedor no puede ser nulo");            
            var supermercadoExistente = await _supermercadoRepository.GetByNameAsync(dto.SupermercadoNombre);            
            ProveedorCore proveedor;
            if (supermercadoExistente != null)
            {
                if (supermercadoExistente.Proveedor != null) throw new SupermercadoException("El supermercado que desea asignarse ya esta vinculado a otra cuenta de proveedor");
                proveedor = MapperProveedor.ToProveedor(dto, supermercadoExistente);
            }
            else
            {
                Supermercado nuevoSupermercado = new Supermercado(dto.SupermercadoNombre);
                proveedor = MapperProveedor.ToProveedor(dto, nuevoSupermercado);                
            }                
            var proveedorExistente = await _proveedorRepository.GetByEmailAsync(dto.Email);
            if (proveedorExistente != null) throw new Exception("Ya existe un proveedor con ese email");
            await _proveedorRepository.AddAsync(proveedor);
        }
    }
}
