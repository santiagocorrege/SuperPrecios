using SuperPrecios.Application.DTO.Miembro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Miembro
{
    public interface IMiembroAddService
    {
        public void Run(DtoMiembroAdd dto);
    }
}
