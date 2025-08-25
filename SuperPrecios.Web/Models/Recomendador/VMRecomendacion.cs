// VMRecomendacion.cs
using SuperPrecios.Application.DTO.Recomendador;

namespace SuperPrecios.Web.Models.Recomendador
{
    public class VMRecomendacion
    {
        public VMSupermercadoGanador SupermercadoGanador { get; set; }
        public List<VMSupermercadoComparativo> SupermercadosComparativos { get; set; }
        public List<VMProductoComparativo> ProductosComparativos { get; set; }
        public decimal AhorroTotal { get; set; }
        public int CantidadProductos { get; set; }

        public static VMRecomendacion FromDto(DtoBestPreciosSupermercado dto)
        {
            var ganador = new VMSupermercadoGanador
            {
                Nombre = dto.SupermercadoGanadorNombre,
                Total = dto.DtoPreciosGanadores.Sum(p => p.Precio * p.Cantidad),
                Productos = dto.DtoPreciosGanadores.Select(p => new VMProductoPrecio
                {
                    NombreCompleto = $"{p.DtoProducto.Nombre} - {p.DtoProducto.Marca.Nombre}",
                    Precio = p.Precio,
                    Cantidad = p.Cantidad,
                    Subtotal = p.Precio * p.Cantidad,
                    ImagenUrl = null // No disponible en el DTO actual
                }).ToList()
            };

            var comparativos = dto.DtoBestPreciosSupermercadosPerdedores.Select(perdedor =>
                new VMSupermercadoComparativo
                {
                    Nombre = perdedor.SupermercadoNombre,
                    Total = perdedor.DtoPrecios.Sum(p => p.Precio * p.Cantidad),
                    DiferenciaConGanador = perdedor.DtoPrecios.Sum(p => p.Precio * p.Cantidad) - ganador.Total,
                    Productos = perdedor.DtoPrecios.Select(p => new VMProductoPrecio
                    {
                        NombreCompleto = $"{p.DtoProducto.Nombre} - {p.DtoProducto.Marca.Nombre}",
                        Precio = p.Precio,
                        Cantidad = p.Cantidad,
                        Subtotal = p.Precio * p.Cantidad,
                        ImagenUrl = null // No disponible en el DTO actual
                    }).ToList()
                }).OrderBy(c => c.Total).ToList();

            // Crear comparativo por productos
            var productosComparativos = CreateProductosComparativos(dto);

            return new VMRecomendacion
            {
                SupermercadoGanador = ganador,
                SupermercadosComparativos = comparativos,
                ProductosComparativos = productosComparativos,
                AhorroTotal = comparativos.Any() ? comparativos.First().DiferenciaConGanador : 0,
                CantidadProductos = dto.DtoPreciosGanadores.Count
            };
        }

        private static List<VMProductoComparativo> CreateProductosComparativos(DtoBestPreciosSupermercado dto)
        {
            var productosComparativos = new List<VMProductoComparativo>();

            foreach (var productoGanador in dto.DtoPreciosGanadores)
            {
                var comparativo = new VMProductoComparativo
                {
                    NombreCompleto = $"{productoGanador.DtoProducto.Nombre} - {productoGanador.DtoProducto.Marca.Nombre}",
                    Cantidad = productoGanador.Cantidad,
                    SupermercadoGanador = new VMPrecioSupermercado
                    {
                        Nombre = dto.SupermercadoGanadorNombre,
                        PrecioUnitario = productoGanador.Precio,
                        Subtotal = productoGanador.Precio * productoGanador.Cantidad
                    },
                    OtrosSupermercados = new List<VMPrecioSupermercado>()
                };

                // Buscar el mismo producto en otros supermercados
                foreach (var perdedor in dto.DtoBestPreciosSupermercadosPerdedores)
                {
                    var productoEnPerdedor = perdedor.DtoPrecios
                        .FirstOrDefault(p => p.DtoProducto.Id == productoGanador.DtoProducto.Id);

                    if (productoEnPerdedor != null)
                    {
                        comparativo.OtrosSupermercados.Add(new VMPrecioSupermercado
                        {
                            Nombre = perdedor.SupermercadoNombre,
                            PrecioUnitario = productoEnPerdedor.Precio,
                            Subtotal = productoEnPerdedor.Precio * productoEnPerdedor.Cantidad
                        });
                    }
                }

                // Ordenar otros supermercados por precio unitario
                comparativo.OtrosSupermercados = comparativo.OtrosSupermercados
                    .OrderBy(s => s.PrecioUnitario).ToList();

                productosComparativos.Add(comparativo);
            }

            return productosComparativos;
        }
    }

    public class VMSupermercadoGanador
    {
        public string Nombre { get; set; }
        public decimal Total { get; set; }
        public List<VMProductoPrecio> Productos { get; set; }
    }

    public class VMSupermercadoComparativo
    {
        public string Nombre { get; set; }
        public decimal Total { get; set; }
        public decimal DiferenciaConGanador { get; set; }
        public List<VMProductoPrecio> Productos { get; set; }

        public string PorcentajeDiferencia => DiferenciaConGanador > 0 && Total > 0
            ? $"+{(DiferenciaConGanador / (Total - DiferenciaConGanador) * 100):F1}%"
            : "0%";
    }

    public class VMProductoPrecio
    {
        public string NombreCompleto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string? ImagenUrl { get; set; }
    }

    // Nuevas clases para el desglose comparativo
    public class VMProductoComparativo
    {
        public string NombreCompleto { get; set; }
        public int Cantidad { get; set; }
        public VMPrecioSupermercado SupermercadoGanador { get; set; }
        public List<VMPrecioSupermercado> OtrosSupermercados { get; set; }
    }

    public class VMPrecioSupermercado
    {
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public decimal DiferenciaConMejor(decimal mejorSubtotal)
        {
            return Subtotal - mejorSubtotal;
        }

        public string PorcentajeDiferenciaConMejor(decimal mejorSubtotal)
        {
            if (mejorSubtotal <= 0) return "0%";
            var diferencia = DiferenciaConMejor(mejorSubtotal);
            return diferencia > 0 ? $"+{(diferencia / mejorSubtotal * 100):F1}%" : "0%";
        }
    }
}