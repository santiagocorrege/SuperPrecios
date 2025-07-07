using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Proveedor;
using ProveedorCore = SuperPrecios.Domain.Entities.Proveedor;
using SupermercadoCore = SuperPrecios.Domain.Entities.Supermercado;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;

namespace SuperPrecios.Application.Services.Proveedor
{
    public class ProveedorAddService : IProveedorAddService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ISupermercadoRepository _SupermercadoRepository;

        public ProveedorAddService(IProveedorRepository proveedorRepo, ISupermercadoRepository SupermercadoRepository)
        {
            _proveedorRepository = proveedorRepo;
            _SupermercadoRepository = SupermercadoRepository;

        }

        public async Task Run(DtoProveedorAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("El proveedor no puede ser nulo");            
            var SupermercadoExistente = await _SupermercadoRepository.GetByNameAsync(dto.SupermercadoNombre);            
            ProveedorCore proveedor;
            if (SupermercadoExistente != null) throw new SupermercadoException("El Supermercado que desea agregar ya esta registrado");
            SupermercadoCore nuevoSupermercado = new SupermercadoCore(dto.SupermercadoNombre);
            proveedor = MapperProveedor.ToProveedor(dto, nuevoSupermercado);                                        
            var proveedorExistente = await _proveedorRepository.GetByEmailAsync(dto.Email);
            if (proveedorExistente != null) throw new Exception("Ya existe un proveedor con ese email");
            await _proveedorRepository.AddAsync(proveedor);
        }
    }
}
