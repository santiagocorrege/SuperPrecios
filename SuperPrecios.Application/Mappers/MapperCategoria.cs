using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperCategoria
    {
        public static DtoCategoriaGet ToDto(Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentNullException("MapperError: La categoria no puede ser nula");
            }
            return new DtoCategoriaGet
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre
            };
        }

        public static Categoria ToCategoria(DtoCategoriaAdd dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("MapperError: El dto de categoria no puede ser nulo");
            }
            return new Categoria(dto.Nombre);
        }
    }
}
