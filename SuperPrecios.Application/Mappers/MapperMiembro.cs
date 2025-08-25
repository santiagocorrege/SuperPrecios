using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Domain.Entities;

namespace SuperPrecios.Application.Mappers
{
    public class MapperMiembro
    {
        public static DtoMiembroUpdate ToDtoUpdate(Miembro miembro)
        {
            return new DtoMiembroUpdate
            {
                Id = miembro.Id,
                Nombre = miembro.Nombre,
                Apellido = miembro.Apellido,
                Email = miembro.Email.Valor,                
            };
        }
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

        public static Miembro ToMiembro(DtoMiembroUpdate dto)
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

        public static Miembro ToMiembroWOPassword(DtoMiembroUpdate dto)
        {
            var miembro = new Miembro(
                dto.Nombre,
                dto.Apellido,
                dto.Email                
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
