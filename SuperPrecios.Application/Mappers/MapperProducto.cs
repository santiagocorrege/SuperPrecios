using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperProducto
    {
        public static IEnumerable<DtoProductoPreciosHistoricos> ToDtoWPreciosHistoricos(IEnumerable<Producto> productos, string supermercado)
        {
            if (productos == null || String.IsNullOrWhiteSpace(supermercado)) throw new ArgumentNullException("Error mapper: La lista de productos no puede ser nula");

            return productos.Select(p => new DtoProductoPreciosHistoricos
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Marca = MapperMarca.ToDto(p.Marca),
                Categoria = MapperCategoria.ToDto(p.Categoria),
                Supermercado = supermercado,
                PreciosHistoricos = MapperPrecioHistorico.ToDtoWOProductoList(p.PreciosHistoricos)
            });
        }
        public static DtoProductoGet ToDtoCompleto(Producto producto)
        {
            if (producto == null) throw new ArgumentNullException("Error mapper: El producto no puede ser nulo");
            return new DtoProductoGet
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Marca = MapperMarca.ToDto(producto.Marca),
                Categoria = MapperCategoria.ToDto(producto.Categoria)
            };
        }

        public static IEnumerable<DtoProductoGet> ToDtoCompletoList(IEnumerable<Producto> productos)
        {
            return productos.Select(p => ToDtoCompleto(p));
        }
    }
}
