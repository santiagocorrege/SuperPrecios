using SuperPrecios.Application.DTO.Carrito;

namespace SuperPrecios.Web.Models.Carrito
{
    public class VMCarritoJson
    {
        public int CantidadTotal { get; set; }
        public int CantidadTipos { get; set; }
        public decimal Total { get; set; }
        public bool TieneProductos { get; set; }
        public List<VMLineaJson> Lineas { get; set; } = new();
        public DateTime UltimaActualizacion { get; set; }

        public static VMCarritoJson FromDto(DtoCarrito dto)
        {
            return new VMCarritoJson
            {
                CantidadTotal = dto.CantidadTotalProductos,
                CantidadTipos = dto.CantidadTiposProductos,
                Total = dto.Total,
                TieneProductos = dto.TieneProductos,
                UltimaActualizacion = DateTime.Now,
                Lineas = dto.Lineas.Select(l => new VMLineaJson
                {
                    ProductoId = l.ProductoId,
                    Nombre = l.NombreCompleto,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = l.PrecioUnitario,
                    Subtotal = l.Subtotal,
                    ImagenUrl = l.ImagenUrl,
                    SupermercadoMenorPrecio = l.SupermercadoMenorPrecio
                }).ToList()
            };
        }
    }
}
