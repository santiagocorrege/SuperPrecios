using SuperPrecios.Application.DTO.Carrito;

namespace SuperPrecios.Web.Models.Carrito
{
    public class VMLineaCarrito
    {
        public DtoLineaCarrito Linea { get; set; } = new();
        public bool MostrarBotones { get; set; } = true;
        public bool ModoMini { get; set; } = false;

        // Propiedades para controles de cantidad
        public int CantidadMinima => 1;
        public int CantidadMaxima => 1000;
    }
}
