using ProductoCore = SuperPrecios.Domain.Entities.Producto;
using PrecioHistoricoCore = SuperPrecios.Domain.Entities.PrecioHistorico;
using MarcaCore = SuperPrecios.Domain.Entities.Marca;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class EntidadesProcesadasDto
    {
        public List<MarcaCore> MarcasNuevas { get; set; } = new List<MarcaCore>();
        public List<ProductoCore> ProductosNuevos { get; set; } = new List<ProductoCore>();
        public List<PrecioHistoricoCore> PreciosHistoricos { get; set; } = new List<PrecioHistoricoCore>();
    }
}
