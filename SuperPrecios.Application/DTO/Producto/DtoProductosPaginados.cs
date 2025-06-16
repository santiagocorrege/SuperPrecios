using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Producto
{
    public class DtoProductosPaginados
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }

        public IEnumerable<DtoProductoCompleto> DtoProductos { get; set; }
    }
}
