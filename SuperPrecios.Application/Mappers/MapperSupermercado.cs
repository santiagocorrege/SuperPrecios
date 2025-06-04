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
            if (dto == null) throw new ArgumentNullException("Error mapper: El supermercado no puede ser nulo");
            return new Supermercado(dto.Nombre);
        }

        public static DtoSupermercadoGet ToDto (Supermercado supermercado)
        {
            if (supermercado == null) throw new ArgumentNullException("Error mapper: El dto de supermercado no puede ser nulo");
            return new DtoSupermercadoGet
            {
                Id = supermercado.Id,
                Nombre = supermercado.Nombre,
                WebsiteUrl = supermercado.WebsiteUrl
            };
        }
    }
}
