using SuperPrecios.Application.DTO.Miembro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.UCInterfaces.Miembro
{
    public interface IMiembroGetById
    {
        public DtoMiembroUpdate Ejecutar(int id);
    }
}
