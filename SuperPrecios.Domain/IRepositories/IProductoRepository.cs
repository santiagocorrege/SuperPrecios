using SuperPrecios.Application.Common;
using SuperPrecios.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IProductoRepository
    {
        // Métodos CRUD básicos
        Task AddAsync(Producto producto);
        Task DeleteAsync(Producto producto);
        Task UpdateAsync(Producto entity);

        // ✅ NUEVO: Método para inserción en lote (alta performance)
        Task AddRangeAsync(IEnumerable<Producto> productos);

        // Métodos de consulta
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> GetByIdAsync(int id);
        Task<Producto> GetByNombreAsync(string nombreProducto);
        Task<IEnumerable<Producto>> GetProductosByMarca(Marca marca);

        // Métodos especializados con precios históricos
        Task<Producto> GetProductoTodayWPrecioHistorico(int id);
        Task<PagedResult<Producto>> GetProductosTodayWPrecioHistorico(int pagina, int pageSize = 10);
        Task<PagedResult<Producto>> GetProductosByNombreTodayWPrecioHistorico(string nombre, int pagina, int pageSize = 10);
        Task<PagedResult<Producto>> GetByCategoriasWithPrecioHistoricoAsync(IEnumerable<int> categoriaIds, int pagina, int pageSize = 10);
    }
}