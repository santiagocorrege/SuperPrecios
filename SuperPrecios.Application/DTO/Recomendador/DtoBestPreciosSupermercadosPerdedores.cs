using SuperPrecios.Application.DTO.PrecioHistorico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Recomendador
{
    public class DtoBestPreciosSupermercadosPerdedores
    {
        public string SupermercadoNombre { get; set; }

        public List<DtoLineaBestPrecios> DtoPrecios { get; set; }
    }
}
