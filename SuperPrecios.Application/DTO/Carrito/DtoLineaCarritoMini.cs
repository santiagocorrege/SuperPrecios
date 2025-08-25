using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Carrito
{
    public class DtoLineaCarritoMini
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string? MarcaNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? ImagenUrl { get; set; }

        public string NombreCompleto => !string.IsNullOrEmpty(MarcaNombre)
            ? $"{MarcaNombre} - {NombreProducto}"
            : NombreProducto;
    }
}
