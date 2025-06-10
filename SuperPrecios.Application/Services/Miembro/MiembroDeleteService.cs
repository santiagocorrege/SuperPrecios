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
            if(id < 1) throw new ArgumentNullException("El id no puede ser menor a 0");
            var miembro = await _miembroRepository.GetByIdAsync(id);
            if (miembro == null) throw new KeyNotFoundException("El miembro que se desea eliminar no existe");
            await _miembroRepository.DeleteAsync(miembro);
        }
    }
}
