using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entities.ValueObject
{
    public record LineaBestPrecios
    {
        public PrecioHistorico PrecioHistorico { get; set; }

        public int Cantidad { get; set; }

        public LineaBestPrecios(PrecioHistorico ph, int cantidad)
        {
            Cantidad = cantidad;
            PrecioHistorico = ph;
        }

        public decimal TotalLinea()
        {
            return PrecioHistorico.Precio * Cantidad;
        }
    }
}
