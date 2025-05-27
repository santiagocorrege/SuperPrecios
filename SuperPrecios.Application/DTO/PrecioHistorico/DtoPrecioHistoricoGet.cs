using SuperPrecios.Application.DTO.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.PrecioHistorico
{
    public class DtoPrecioHistoricoGet
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal Precio { get; set; }
        public DtoProductoGet DtoProducto { get; set; }
        
    }
}
