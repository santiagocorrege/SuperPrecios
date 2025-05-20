using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.AutenticacionCore.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MAPPERS
{
    public class MapperMiembro
    {
        public DtoMiembroAdd ToDtoMiembroAdd(SuperPrecios.AutenticacionCore.Entidades.Miembro miembro)
        {
            return new DtoMiembroAdd
            {

            };
        }
    }
}
