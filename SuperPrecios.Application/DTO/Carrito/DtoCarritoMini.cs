using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Carrito
{
    public class DtoCarritoMini
    {
        public int CantidadTotalProductos { get; set; }
        public int CantidadTiposProductos { get; set; }
        public decimal Total { get; set; }
        public bool TieneProductos { get; set; }

        // Últimos productos agregados (para preview)
        public List<DtoLineaCarritoMini> UltimasLineas { get; set; } = new();

        // Factory method para crear desde DtoCarrito
        public static DtoCarritoMini FromCarrito(DtoCarrito carrito)
        {
            return new DtoCarritoMini
            {
                CantidadTotalProductos = carrito.CantidadTotalProductos,
                CantidadTiposProductos = carrito.CantidadTiposProductos,
                Total = carrito.Total,
                TieneProductos = carrito.TieneProductos,
                UltimasLineas = carrito.Lineas
                    .OrderByDescending(l => l.ProductoId) // Asumiendo que ID mayor = más reciente
                    .Take(3)
                    .Select(l => new DtoLineaCarritoMini
                    {
                        ProductoId = l.ProductoId,
                        NombreProducto = l.NombreProducto,
                        MarcaNombre = l.MarcaNombre,
                        Cantidad = l.Cantidad,
                        PrecioUnitario = l.PrecioUnitario,
                        ImagenUrl = l.ImagenUrl
                    })
                    .ToList()
            };
        }
    }
}
