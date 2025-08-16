using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class MatchingTransactionDto
    {
        public List<Domain.Entities.Marca> MarcasNuevas { get; set; } = new();
        public List<Domain.Entities.Producto> ProductosNuevos { get; set; } = new();
        public List<Domain.Entities.PrecioHistorico> PreciosHistoricos { get; set; } = new();
    }
}
