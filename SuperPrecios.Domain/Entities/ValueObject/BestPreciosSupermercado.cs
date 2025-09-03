using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entities.ValueObject
{
    public record BestPreciosSupermercado
    {
        #region Properties
        public Supermercado Supermercado { get; set; }        
        
        public List<LineaBestPrecios> Lineas { get; set; } = new List<LineaBestPrecios>();
        
        public decimal CostoTotalProductos { get; init; }

        public BestPreciosSupermercado(Supermercado supermercado, List<LineaBestPrecios> lineas)
        {
            Supermercado = supermercado;
            Lineas = lineas;
            CostoTotalProductos = CostoTotalProductosSupermercado();
        }

        #endregion
        #region Methods
        private decimal CostoTotalProductosSupermercado()
        {
            decimal total = 0;
            foreach(LineaBestPrecios l in Lineas)
            {
                total += l.TotalLinea();
            }
            return total;
        }
        #endregion
    }
}
