using SuperPrecios.Application.DTO.Carrito;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperCarrito
    {
        public static DtoCarrito ToDto(Carrito carrito)
        {
            if (carrito == null) throw new ArgumentNullException("MapperError: El carrito no puede ser nulo");

            return new DtoCarrito
            {
                Id = carrito.Id,
                UsuarioId = carrito.UsuarioId,
                NombreUsuario = carrito.Usuario?.Nombre ?? string.Empty,
                FechaCreacion = carrito.FechaCreacion,
                Lineas = carrito.Lineas?.Select(l => ToLineaDto(l)).ToList() ?? new List<DtoLineaCarrito>()
            };
        }

        public static DtoLineaCarrito ToLineaDto(Linea linea)
        {
            if (linea == null) throw new ArgumentNullException("MapperError: La línea del carrito no puede ser nula");

            // Obtener el precio más bajo disponible hoy
            var precioMasBajo = linea.Producto?.PreciosHistoricos?
                .Where(ph => ph.Fecha == TimeHelper.DateOnlyNowInMontevideo())
                .OrderBy(ph => ph.Precio)
                .FirstOrDefault();

            return new DtoLineaCarrito
            {
                ProductoId = linea.ProductoId,
                Cantidad = linea.Cantidad,
                NombreProducto = linea.Producto?.Nombre ?? string.Empty,
                MarcaNombre = linea.Producto?.Marca?.Nombre,
                ImagenUrl = linea.Producto?.ImgUrl,
                CategoriaNombre = linea.Producto?.Categoria?.Nombre,
                PrecioUnitario = precioMasBajo?.Precio ?? 0m,
                SupermercadoMenorPrecio = precioMasBajo?.Supermercado?.Nombre
            };
        }

        public static DtoCarritoMini ToCarritoMini(Carrito carrito)
        {
            if (carrito == null) throw new ArgumentNullException("MapperError: El carrito no puede ser nulo");

            var carritoDto = ToDto(carrito);
            return DtoCarritoMini.FromCarrito(carritoDto);
        }

        public static IEnumerable<DtoCarrito> ToDto(IEnumerable<Carrito> carritos)
        {
            return carritos.Select(c => ToDto(c));
        }
    }
}
