using SuperPrecios.Application.Common;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IProductoRepository : IRepository<Producto>
    {
        public Task<Producto> GetByNombreAsync(string nombreProducto);
        public Task<IEnumerable<Producto>> GetProductosByMarca (Marca marca);        
        public Task<Producto> GetProductoTodayWPrecioHistorico(int id);
        public Task<PagedResult<Producto>> GetProductosTodayWPrecioHistorico(int pagina, int pageSize);        
        public Task<PagedResult<Producto>> GetProductosByNombreTodayWPrecioHistorico(string nombre, int pagina, int pageSize);        
        public Task<PagedResult<Producto>> GetByCategoriasWithPrecioHistoricoAsync(IEnumerable<int> categoriaIds, int pagina, int pageSize);

    }
}
