using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.PrecioHistorico
{
    public class DtoPrecioHistoricoWOProducto
    {
        public DateOnly Fecha { get; set; }
        public decimal Precio { get; set; }
    }
}
