using SuperPrecios.Application.DTO.PrecioHistorico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Recomendador
{
    public class DtoBestPreciosSupermercado
    {
        public string SupermercadoGanadorNombre { get; set; }     
        
        public List<DtoLineaBestPrecios> DtoPreciosGanadores { get; set; }
        public List<DtoBestPreciosSupermercadosPerdedores> DtoBestPreciosSupermercadosPerdedores { get; set; }
    }
}
