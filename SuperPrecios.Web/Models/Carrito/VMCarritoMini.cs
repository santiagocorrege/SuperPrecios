using SuperPrecios.Application.DTO.Carrito;

namespace SuperPrecios.Web.Models.Carrito
{
    public class VMCarritoMini
    {
        public DtoCarritoMini Carrito { get; set; } = new();
        public bool MostrarDropdown { get; set; } = true;

        // Propiedades de conveniencia para la vista
        public string TextoCantidad => Carrito.TieneProductos
            ? $"{Carrito.CantidadTotalProductos}"
            : "0";

        public string TextoTotal => Carrito.TieneProductos
            ? $"${Carrito.Total:N2}"
            : "$0.00";

        public string CssClassBadge => Carrito.TieneProductos
            ? "badge bg-danger"
            : "badge bg-secondary";
    }
}
