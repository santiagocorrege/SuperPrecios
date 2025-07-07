using SuperPrecios.Application.DTO.Supermercado;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperSupermercado
    {
        public static Supermercado ToSupermercado(DtoSupermercadoAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("Error mapper: El Supermercado no puede ser nulo");
            return new Supermercado(dto.Nombre);
        }

        public static DtoSupermercadoGet ToDto (Supermercado Supermercado)
        {
            if (Supermercado == null) throw new ArgumentNullException("Error mapper: El dto de Supermercado no puede ser nulo");
            return new DtoSupermercadoGet
            {
                Id = Supermercado.Id,
                Nombre = Supermercado.Nombre,
                WebsiteUrl = Supermercado.WebsiteUrl
            };
        }

        public static IEnumerable<DtoSupermercadoGet> ToDto (IEnumerable<Supermercado> Supermercados)
        {
            return Supermercados.Select(s => ToDto(s));
        }
    }
}
