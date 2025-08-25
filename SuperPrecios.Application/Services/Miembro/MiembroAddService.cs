using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Miembro;
using MiembroCore = SuperPrecios.Domain.Entities.Miembro;
using CarritoCore = SuperPrecios.Domain.Entities.Carrito;

namespace SuperPrecios.Application.Services.Miembro
{
    public class MiembroAddService : IMiembroAddService
    {
        private IMiembroRepository _miembroRepo;

        public MiembroAddService(IMiembroRepository miembroRepo)
        {
            _miembroRepo = miembroRepo;
        }
        public async Task Run(DtoMiembroAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("El miembro no puede ser nulo");

            MiembroCore miembroBuscado = await _miembroRepo.GetByEmailAsync(dto.Email);
            if (miembroBuscado != null) throw new Exception("El miembro que se desea agregar ya existe con ese email");

            MiembroCore miembro = MapperMiembro.ToMiembro(dto);

            // Paso 1: Guardar miembro sin carrito
            await _miembroRepo.AddAsync(miembro);

            // Paso 2: Crear carrito usando el ID del miembro ya persistido
            CarritoCore carrito = new CarritoCore(miembro.Id);
            miembro.Carrito = carrito;

            // Paso 3: Actualizar miembro con carrito (si es necesario)
            await _miembroRepo.UpdateAsync(miembro);
        }
    }
}
