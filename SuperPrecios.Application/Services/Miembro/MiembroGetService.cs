using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.Mappers;
using SuperPrecios.AutenticacionCore.ValueObject;
using SuperPrecios.AutenticationCore.Exceptions;
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
        public async Task<DtoMiembroGet> Run(string email)
        {
            if(email.Trim().Length == 0)
            {
                throw new MiembroException("El email no puede ser nulo");
            }
            var miembro = await _miembroRepo.GetByEmailAsync(email);
            if(miembro == null)
            {
                throw new MiembroException("El miembro no existe");
            }
            return MapperMiembro.ToDto(miembro);
        }

        public async Task<DtoMiembroGet> Run(int id)
        {
            if (id <= 0)
            {
                throw new MiembroException("El id no puede ser nulo");
            }
            var miembro = await _miembroRepo.GetByIdAsync(id);
            if (miembro == null)
            {
                throw new MiembroException("El miembro no existe");
            }
            return MapperMiembro.ToDto(miembro);
        }
    }
}
