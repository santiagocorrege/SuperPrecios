using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperPrecioHistorico
    {
        public static DtoPrecioHistoricoGet ToDtoCompleto(PrecioHistorico precioHistorico)
        {
            if (precioHistorico == null)
            {
                throw new ArgumentNullException("MapperError: El precio historico no puede ser nulo");
            }
            DtoProductoGet dtoProd = MapperProducto.ToDtoCompleto(precioHistorico.Producto);
            if (dtoProd == null)
            {
                throw new ArgumentNullException("MapperError: El producto del precio historico no puede ser nulo");
            }
            return new DtoPrecioHistoricoGet
            {                
                Fecha = precioHistorico.Fecha,
                Precio = precioHistorico.Precio,
                DtoProducto = dtoProd,
            };
        }

        public static PrecioHistorico ToPrecioHistorico(DtoPrecioHistoricoAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("MapperError: El precio historico no puede ser nulo");
            if(dto.Precio <= 0) throw new ArgumentException("MapperError: El precio no puede ser menor o igual a 0");
            if (dto.DtoProductoAdd == null) throw new ArgumentNullException("MapperError: El producto del precio historico no puede ser nulo");
            Marca marca = MapperMarca.ToMarca(dto.DtoProductoAdd.DtoMarcaAdd);
            Categoria categoria = MapperCategoria.ToCategoria(dto.DtoProductoAdd.DtoCategoriaAdd);
            Producto producto = new Producto(dto.DtoProductoAdd.Nombre, marca, categoria);
            return new PrecioHistorico(producto, supermercado, dto.Precio);
            

        }

    }
}
