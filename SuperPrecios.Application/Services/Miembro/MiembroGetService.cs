using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.Mappers;
using SuperPrecios.AuthenticationCore.ValueObject;
using SuperPrecios.AuthenticationCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Miembro
{
    public class MiembroGetService : IMiembroGet
    {
        private readonly IMiembroRepository _miembroRepo;

        public MiembroGetService(IMiembroRepository miembroRepo)
        {
            _miembroRepo = miembroRepo;
        }
        public async Task<IEnumerable<DtoMiembroGet>> Run()
        {
            var dtoMiembro = await _miembroRepo.GetAllAsync();
            return MapperMiembro.ToDto(dtoMiembro);
        }

        public async Task<IEnumerable<DtoMiembroGet>> RunByEmailList(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new MiembroException("El email no puede ser nulo");
            }
            var dtoListaMiembro = await _miembroRepo.GetByEmailListAsync(email);
            return MapperMiembro.ToDto(dtoListaMiembro);
        }

        public async Task<DtoMiembroGet> RunByEmail(string email)
        {
            if(String.IsNullOrWhiteSpace(email))
            {
                throw new MiembroException("El email no puede ser nulo");
            }
            var miembroBuscado = await _miembroRepo.GetByEmailAsync(email);
            if(miembroBuscado == null) throw new MiembroException("No existe miembro con ese email");            
            return MapperMiembro.ToDto(miembroBuscado);
        }

        public async Task<DtoMiembroGet> RunById(int id)
        {
            if (id <= 0) throw new MiembroException("El id no puede ser nulo");            
            var miembroBuscado = await _miembroRepo.GetByIdAsync(id);
            if(miembroBuscado == null) throw new MiembroException("No existe miembro con ese id");
            return MapperMiembro.ToDto(miembroBuscado);
        }

        public async Task<DtoMiembroUpdate> RunGetUpdate(int id)
        {
            if (id < 1) throw new MiembroException("El id no puede ser nulo");            
            var miembroBuscado = await _miembroRepo.GetByIdAsync(id);
            if(miembroBuscado == null) throw new MiembroException("No existe miembro con ese id");
            return MapperMiembro.ToDtoUpdate(miembroBuscado);
        }
    }
}
