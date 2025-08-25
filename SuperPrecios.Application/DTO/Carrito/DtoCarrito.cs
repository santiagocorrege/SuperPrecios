using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Carrito
{
    public class DtoCarrito
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public List<DtoLineaCarrito> Lineas { get; set; } = new();

        // Propiedades calculadas
        public int CantidadTotalProductos => Lineas.Sum(l => l.Cantidad);
        public int CantidadTiposProductos => Lineas.Count;
        public decimal Total => Lineas.Sum(l => l.Subtotal);
        public bool TieneProductos => Lineas.Any();
    }
}
