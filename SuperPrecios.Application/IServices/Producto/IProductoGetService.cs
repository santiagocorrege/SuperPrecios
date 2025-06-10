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
    }
}
