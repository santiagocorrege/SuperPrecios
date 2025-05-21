using SuperPrecios.Application.DTO.Miembro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Miembro
{
    public interface IMiembroGet
    {
        public Task<DtoMiembroGet> Run(string email);

        public Task<DtoMiembroGet> Run(int id);
    }
}
