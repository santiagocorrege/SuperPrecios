using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.Domain.IRepositories;
using ProveedorCore = SuperPrecios.Domain.Entities.Proveedor;
using SuperPrecios.Application.Mappers;
using SuperPrecios.AuthenticationCore.Exceptions;
using SuperPrecios.Application.IServices.Proveedor;
using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Domain.Exceptions;

namespace SuperPrecios.Application.Services.Proveedor
{
    public class ProveedorUpdateService : IProveedorUpdateService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ISupermercadoRepository _supermercadoRepository;

        public ProveedorUpdateService(IProveedorRepository proveedorRepository, ISupermercadoRepository supermercadoRepository)
        {
            _proveedorRepository = proveedorRepository;
            _supermercadoRepository = supermercadoRepository;
        }
        public async Task Run(DtoProveedorUpdate dto)
        {
            if (dto == null || dto.Id < 1) throw new ArgumentNullException("El id del proveedor que desea actualizar no es valido");                     
            ProveedorCore proveedorActualizado;
            if (dto.Password != null)
            {
                proveedorActualizado = MapperProveedor.ToProveedor(dto);
            }
            else
            {
                proveedorActualizado = MapperProveedor.ToProveedorWOPassword(dto);
            }
            ProveedorCore proveedorBuscado = await _proveedorRepository.GetByIdAsync(dto.Id);
            if (proveedorBuscado == null) throw new MiembroException("El miembro que desea actualizar no existe");
            proveedorBuscado.Modificar(proveedorActualizado);
            await _proveedorRepository.UpdateAsync(proveedorBuscado);
        }
    }
}
