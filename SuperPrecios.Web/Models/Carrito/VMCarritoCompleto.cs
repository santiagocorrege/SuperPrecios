using SuperPrecios.Application.DTO.Carrito;

namespace SuperPrecios.Web.Models.Carrito
{
    public class VMCarritoCompleto
    {
        public DtoCarrito Carrito { get; set; } = new();
        public string? MensajeInfo { get; set; }

        // Propiedades de conveniencia
        public bool TieneProductos => Carrito.TieneProductos;
        public decimal Total => Carrito.Total;
        public int CantidadProductos => Carrito.CantidadTotalProductos;
    }
}
