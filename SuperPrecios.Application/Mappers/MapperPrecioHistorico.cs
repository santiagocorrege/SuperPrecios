using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static DtoPrecioHistoricoWOProducto ToDtoCompletoWoProducto(PrecioHistorico ph)
        {
            return new DtoPrecioHistoricoWOProducto
            {
                Fecha = ph.Fecha,
                Precio = ph.Precio,
                Supermercado = MapperSupermercado.ToDto(ph.Supermercado)
            };
        }

        public static IEnumerable<DtoPrecioHistoricoWOProducto> ToDtoCompletoWoProducto(IEnumerable<PrecioHistorico> ph)
        {
            return ph.Select(p => ToDtoCompletoWoProducto(p));
        }

        public static DtoPrecioHistoricoWOProductoWOSupermercado ToDtoWOProductoWOSupermercado(PrecioHistorico precioHistorico)
        {
            if (precioHistorico == null)
            {
                throw new ArgumentNullException("MapperError: El precio historico no puede ser nulo");
            }
            return new DtoPrecioHistoricoWOProductoWOSupermercado
            {
                Fecha = precioHistorico.Fecha,
                Precio = precioHistorico.Precio,                
            };
        }

        public static IEnumerable<DtoPrecioHistoricoWOProductoWOSupermercado> ToDtoWOProductoWOSupermercadoList(IEnumerable<PrecioHistorico> preciosHistoricos)
        {
            return preciosHistoricos.Select(p => ToDtoWOProductoWOSupermercado(p));
        }
        public static PrecioHistorico ToPrecioHistorico(DtoPrecioHistoricoAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("El precio historico no puede ser nulo");
            if (dto.Precio <= 0) throw new ArgumentException("El precio no puede ser menor a 0");
            if (dto.SupermercadoId <= 0) throw new ArgumentException("El id del supermercado no es valido");
            Categoria categoria = new Categoria(dto.Categoria);
            Marca marca = new Marca(dto.Marca);
            Producto producto = new Producto(dto.Producto, marca, categoria);
            PrecioHistorico precioHistorico = new PrecioHistorico(producto, dto.SupermercadoId, dto.Precio);
            return precioHistorico;
        }

        public static IEnumerable<PrecioHistorico> ToPrecioHistoricoList(List<DtoPrecioHistoricoAdd> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0) throw new ArgumentException("La lista de precios historicos no puede ser nula o vacía.", nameof(dtoList));
            return dtoList.Select(p => ToPrecioHistorico(p));
        }


    }
}
