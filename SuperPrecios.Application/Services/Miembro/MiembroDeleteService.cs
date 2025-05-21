using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Miembro
{
    public class MiembroDeleteService : IMiembroDeleteService
    {
        private readonly IMiembroRepository _miembroRepository;
        public MiembroDeleteService(IMiembroRepository repo)
        {
            _miembroRepository = repo;
        }
        public async Task Run(int id)
        {
            if(id <= 0) {
                throw new ArgumentNullException("El id no puede ser menor o igual a 0");
            }
            _miembroRepository.DeleteAsync(id);
        }
    }
}
