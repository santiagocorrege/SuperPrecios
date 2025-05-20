using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.AutenticacionCore.Entities.Miembro;
using SuperPrecios.Application.Mappers;
using SuperPrecios.AutenticationCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Miembro
{
    public class MiembroUpdateService : IMiembroUpdateService
    {
        private readonly IMiembroRepository _miembroRepository;

        public MiembroUpdateService(IMiembroRepository miembroRepository)
        {
            _miembroRepository = miembroRepository;
        }
        public void Ejecutar(DtoMiembroUpdate dto)
        {
            if(dto == null || dto.Id <= 0)
            {
                throw new ArgumentNullException("El miembro que desea actualizar no puede ser nulo");
            }
            SuperPrecios.AutenticacionCore.Entities.Miembro miembroBuscado = _miembroRepository.GetById(dto.Id);
            if(miembroBuscado == null)
            {
                throw new MiembroException("El miembro que desea actualizar no existe");
            }
            Miembro miembro = MapperMiembro.ToDto(dto);

            _miembroRepository.Update();
        }
    }
}
