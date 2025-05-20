using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Application.UCInterfaces.Miembro;
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
        public void Ejecutar(DtoMiembroAdd dto)
        {
            if(dto == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            _miembroRepo.Add(MapperMiembro.ToMiembro(dto));
        }
    }
}
