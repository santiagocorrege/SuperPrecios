namespace SuperPrecios.Web.Models.Carrito
{
    public class VMLineaJson
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? ImagenUrl { get; set; }
        public string? SupermercadoMenorPrecio { get; set; }
    }
}
