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
            if(miembro == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
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
            if(dto == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            return new Miembro(
                dto.Nombre,
                dto.Apellido,
                dto.Email,
                dto.Password
            );
        }

        public static Miembro ToDto(DtoMiembroUpdate dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("El miembro no puede ser nulo");
            }
            var miembro = new Miembro(
                dto.Nombre,
                dto.Apellido,
                dto.Email,
                dto.Password
            );
            miembro.Id = dto.Id;
            return miembro;
        }


    }
}
