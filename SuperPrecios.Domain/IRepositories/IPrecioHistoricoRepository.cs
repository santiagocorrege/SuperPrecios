using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IPrecioHistoricoRepository
    {
        public Task<List<PrecioHistorico>> GetPreciosHistoricosBySupermercado(
            int supermercadoId,
            List<Producto> productos);

        public Task<List<Supermercado>> GetSupermercadosFilterPreciosHistoricos(List<Producto> productos);
    }
}
