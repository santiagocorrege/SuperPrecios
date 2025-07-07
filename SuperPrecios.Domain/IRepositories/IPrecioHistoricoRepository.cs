using SuperPrecios.Application.IRepository;
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
        
        public Task AddAsync(PrecioHistorico entity);

        public Task AddAsyncBySupermercadoAndCategoria(IEnumerable<PrecioHistorico> entity, Supermercado supermercado, Categoria categoria);

        public Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, int productoId );

        public Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, string productoNombre);

        public Task<IEnumerable<Producto>> GetAllBySupermercado(int supermercadoId);

    }
}
