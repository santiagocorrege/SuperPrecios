using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Producto
{
    public class ProductoAddService : IProductoAddService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoAddService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public Task AddAsync(DtoProductoAdd productoAdd)
        {
            throw new NotImplementedException();
        }
    }
}
