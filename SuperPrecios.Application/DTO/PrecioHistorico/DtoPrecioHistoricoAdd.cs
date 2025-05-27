using SuperPrecios.Application.DTO.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.PrecioHistorico
{
    public class DtoPrecioHistoricoAdd
    {
        public DtoProductoAdd DtoProductoAdd { get; set; }

        public int SupermercadoId { get; set; }
        public decimal Precio { get; set; }
    }
}
