using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.DTO.Supermercado;
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

        public DtoSupermercadoAdd DtoSupermercadoAdd { get; set; }
        public decimal Precio { get; set; }
    }
}
