using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Carrito
{
    public class DtoLineaCarrito
    {
        // Información de la línea
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        // Información del producto
        public string NombreProducto { get; set; } = string.Empty;
        public string? MarcaNombre { get; set; }
        public string? ImagenUrl { get; set; }
        public string? CategoriaNombre { get; set; }

        // Precio actual (el menor disponible hoy)
        public decimal PrecioUnitario { get; set; }
        public string? SupermercadoMenorPrecio { get; set; }

        // Propiedades calculadas
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public string NombreCompleto => !string.IsNullOrEmpty(MarcaNombre)
            ? $"{MarcaNombre} - {NombreProducto}"
            : NombreProducto;
    }
}
