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

        public Task AddAsyncCompleto(Producto producto);
        
    }
}
