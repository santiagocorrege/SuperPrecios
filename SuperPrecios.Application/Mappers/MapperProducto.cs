using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperProducto
    {
        public static DtoProductoGet ToDtoCompleto(Producto producto)
        {
            if (producto == null || producto.Marca == null || producto.Categoria == null)
            {
                throw new ArgumentNullException("MapperError: El producto y sus prop de navegacion no pueden ser nulas");
            }
            //TODO: Cuidado si no se incluye la categoria y marca en el repositorio de producto (Includes)?
            DtoMarcaGet dtoMarca = MapperMarca.ToDto(producto.Marca);                        
            DtoCategoriaGet dtoCategoria = MapperCategoria.ToDto(producto.Categoria);            

            return new DtoProductoGet
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Marca = dtoMarca,
                Categoria = dtoCategoria
            };
        }

        public static Producto ToProducto(DtoProductoAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("MapperError: El producto no puede ser nulo");
            if (dto.DtoMarcaAdd == null) throw new ArgumentNullException("MapperError: La marca no puede ser nula");
            if (dto.DtoCategoriaAdd == null) throw new ArgumentNullException("MapperError: La categoria no puede ser nula");            
            Marca marca = MapperMarca.ToMarca(dto.DtoMarcaAdd);
            Categoria categoria = MapperCategoria.ToCategoria(dto.DtoCategoriaAdd);
            return new Producto(dto.Nombre, marca, categoria);
        }
        
        public static IEnumerable<DtoProductoGet> ToDtoCompleto(List<Producto> productos)
        {
            return productos.Select(p => ToDtoCompleto(p));
        }
    }
}
