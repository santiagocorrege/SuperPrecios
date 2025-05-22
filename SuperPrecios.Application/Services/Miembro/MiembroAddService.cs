using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.IServices.Miembro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if(dto == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            await _miembroRepo.AddAsync(MapperMiembro.ToMiembro(dto));
        }
    }
}
