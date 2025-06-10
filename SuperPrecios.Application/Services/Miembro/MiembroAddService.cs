using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Miembro;
using MiembroCore = SuperPrecios.AuthenticationCore.Entities.Miembro;

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
            if(dto == null) throw new ArgumentNullException("El miembro no puede ser nulo");
            MiembroCore miembro = MapperMiembro.ToMiembro(dto);
            MiembroCore miembroBuscado = await _miembroRepo.GetByEmailAsync(dto.Email);
            if (miembroBuscado != null) throw new Exception("El miembro que se desea agregar ya existe con ese email");
            await _miembroRepo.AddAsync(miembro);
        }
    }
}
