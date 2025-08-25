namespace SuperPrecios.Web.Models.Carrito
{
    public class VMCarritoResponse
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public VMCarritoJson? Carrito { get; set; }
    }
}
