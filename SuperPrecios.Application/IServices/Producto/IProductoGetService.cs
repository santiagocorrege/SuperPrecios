using SuperPrecios.Application.DTO.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Producto
{
    public interface IProductoGetService
    {
        public Task<DtoProductoGet> GetByIdAsync(int id);

        public Task<DtoProductoGet> GetByNombreAsync(string nombre);

        public Task<IEnumerable<DtoProductoGet>> GetAllAsync();

        public Task<DtoProductosPaginados> GetProductosTodayWPrecioHistoricoPaginado(int pagina);
        public Task<DtoProductoCompleto> GetCompletoByIdAsync(int id);

        public Task<DtoProductosPaginados> GetProductosByNameTodayWPrecioHistoricoPaginado(string nombre, int pagina);

        public Task<DtoProductosPaginados> GetProductosByCategoriaTodayWPrecioHistoricoPaginado(int categoriaId, int pagina);
    }
}
