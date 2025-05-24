using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using MiembroCore = SuperPrecios.AuthenticationCore.Entities.Miembro;
using SuperPrecios.Application.Mappers;
using SuperPrecios.AuthenticationCore.Exceptions;
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
        public async Task Run(DtoMiembroUpdate dto)
        {
            if(dto == null || dto.Id <= 0)
            {
                throw new ArgumentNullException("El miembro que desea actualizar no puede ser nulo");
            }
            MiembroCore miembroBuscado = await _miembroRepository.GetByIdAsync(dto.Id);
            if(miembroBuscado == null)
            {
                throw new MiembroException("El miembro que desea actualizar no existe");
            }
            MiembroCore miembroActualizado = MapperMiembro.ToMiembro(dto);
            miembroBuscado.Modificar(miembroActualizado);   
            await _miembroRepository.UpdateAsync(miembroBuscado);
        }
    }
}
