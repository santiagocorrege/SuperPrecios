using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Common
{
    public class PagedResult<T>
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public IEnumerable<T> Items { get; set; }

        public string AditionalData { get; set; }
    }
}
