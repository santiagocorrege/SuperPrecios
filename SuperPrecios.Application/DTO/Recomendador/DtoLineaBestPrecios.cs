using SuperPrecios.Application.DTO.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Recomendador
{
    public class DtoLineaBestPrecios
    {
        public DtoProductoGet DtoProducto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }       
    }
}
