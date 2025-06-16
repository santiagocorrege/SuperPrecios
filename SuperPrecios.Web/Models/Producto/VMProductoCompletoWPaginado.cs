using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Web.Models.Shared;

namespace SuperPrecios.Web.Models.Producto
{
    public class VMProductoCompletoWPaginado : VMPaginadoBase
    {
        public IEnumerable<DtoProductoCompleto> Productos { get; set; } = Enumerable.Empty<DtoProductoCompleto>();

    }
}
