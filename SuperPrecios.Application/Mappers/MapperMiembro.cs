using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.AutenticacionCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperMiembro
    {
        public static DtoMiembroGet ToDto(Miembro miembro)
        {
            return new DtoMiembroGet
            {
                Id = miembro.Id,
                Nombre = miembro.Nombre,
                Apellido = miembro.Apellido,
                Email = miembro.Email.Valor,
            };
        }

        public static Miembro ToMiembro(DtoMiembroAdd dto)
        {
            return new Miembro(
                dto.Nombre,
                dto.Apellido,
                dto.Email,
                dto.Password
            );
        }

        public static Miembro ToDto(DtoMiembroUpdate dto)
        {
            var miembro = new Miembro(
                dto.Nombre,
                dto.Apellido,
                dto.Email,
                dto.Password
            );
            miembro.Id = dto.Id;
            return miembro;
        }

        public static IEnumerable<DtoMiembroGet> ToDto(IEnumerable<Miembro> miembros)
        {
            return miembros.Select(m => ToDto(m));
        }

    }
}
