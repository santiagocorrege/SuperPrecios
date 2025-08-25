using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.Domain.IRepositories;
using ProveedorCore = SuperPrecios.Domain.Entities.Proveedor;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Proveedor;
using SuperPrecios.Domain.Exceptions;

namespace SuperPrecios.Application.Services.Proveedor
{
    public class ProveedorUpdateService : IProveedorUpdateService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ISupermercadoRepository _SupermercadoRepository;

        public ProveedorUpdateService(IProveedorRepository proveedorRepository, ISupermercadoRepository SupermercadoRepository)
        {
            _proveedorRepository = proveedorRepository;
            _SupermercadoRepository = SupermercadoRepository;
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
