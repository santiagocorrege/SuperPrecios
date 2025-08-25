using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.DTO.Recomendador;
using SuperPrecios.Application.DTO.Supermercado;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Entities.ValueObject;


namespace SuperPrecios.Application.Mappers
{
    public class MapperSugerenciaService
    {
        public static DtoBestPreciosSupermercado ToDtoResultado(BestPreciosSupermercado ganador, List<BestPreciosSupermercado> perdedores)
        {
            return new DtoBestPreciosSupermercado
            {
                SupermercadoGanadorNombre = ganador.Supermercado.Nombre,
                DtoPreciosGanadores = ToListDtoLineaBestPrecio(ganador.Lineas),
                DtoBestPreciosSupermercadosPerdedores = ToDtoBPSPerdedores(perdedores)
            };
        }

        private static List<DtoLineaBestPrecios> ToListDtoLineaBestPrecio(List<LineaBestPrecios> lineasBP)
        {
            if (lineasBP == null || lineasBP.Count < 0) throw new ArgumentNullException("Lineas invalidas");
            return lineasBP.Select(l => ToDtoLineaBestPrecio(l)).ToList();
        }

        private static DtoLineaBestPrecios ToDtoLineaBestPrecio(LineaBestPrecios lineaBP)
        {
            if (lineaBP == null)
            {
                throw new ArgumentNullException("MapperError: La linea best precio no puede ser nulo");
            }
            DtoProductoGet dtoProd = MapperProducto.ToDtoCompleto(lineaBP.PrecioHistorico.Producto);
            if (dtoProd == null)
            {
                throw new ArgumentNullException("MapperError: El producto del precio historico no puede ser nulo");
            }
            return new DtoLineaBestPrecios
            {
                Cantidad = lineaBP.Cantidad,
                Precio = lineaBP.PrecioHistorico.Precio,
                DtoProducto = dtoProd,
            };
        }

        private static List<DtoBestPreciosSupermercadosPerdedores> ToDtoBPSPerdedores(List<BestPreciosSupermercado> perdedores)
        {
            if(perdedores != null && perdedores.Count > 0)
            {
                return perdedores.Select(p => ToDtoBPSPerdedores(p)).ToList();
            }
            else
            {
                return new List<DtoBestPreciosSupermercadosPerdedores>();                
            }
            
        }

        private static DtoBestPreciosSupermercadosPerdedores ToDtoBPSPerdedores(BestPreciosSupermercado perdedor)
        {
            return new DtoBestPreciosSupermercadosPerdedores
            {
                SupermercadoNombre = perdedor.Supermercado.Nombre,
                DtoPrecios = ToListDtoLineaBestPrecio(perdedor.Lineas),
            };
        }

        public static List<LineaBestPrecios> ToListLineaBestPrecios(Carrito c, List<PrecioHistorico> preciosHistoricos)
        {
            if (c == null || c.Lineas.Count == 0 || preciosHistoricos == null || preciosHistoricos.Count == 0) throw new Exception("Error mapper");
            List<LineaBestPrecios> lineasBP = new List<LineaBestPrecios>();
            foreach (PrecioHistorico ph in preciosHistoricos)
            {
                var lineaCarrito = c.Lineas.FirstOrDefault(l => l.Producto.Id == ph.ProductoId);

                if (lineaCarrito != null)
                {
                    LineaBestPrecios lBP = new LineaBestPrecios(ph, lineaCarrito.Cantidad);
                    lineasBP.Add(lBP);
                }
            }
            return lineasBP;
        }

    }
}
