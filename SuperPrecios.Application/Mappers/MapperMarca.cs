using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperMarca
    {
        public static DtoMarcaGet ToDto(Marca marca)
        {
            if (marca == null) throw new ArgumentNullException("Error mapper: La marca no puede ser nula");
            return new DtoMarcaGet
            {
                Id = marca.Id,
                Nombre = marca.Nombre,
            };
        }

        public static Marca ToMarca(DtoMarcaAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("Error mapper: La marca no puede ser nula");
            return new Marca(dto.Nombre);            
        }

        public static IEnumerable<DtoMarcaGet> ToDto(IEnumerable<Marca> list)
        {
            return list.Select(p => ToDto(p));
        }

    }
}
