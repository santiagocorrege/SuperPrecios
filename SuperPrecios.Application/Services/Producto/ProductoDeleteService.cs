using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;

namespace SuperPrecios.Application.Services.Producto
{
    public class ProductoDeleteService : IProductoDeleteService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoDeleteService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task Run(int idProducto)
        {
            if(idProducto < 1) throw new ArgumentException("Id de producto invalido.");
            ProductoCore productoBuscado = await _productoRepository.GetByIdAsync(idProducto);
            if (productoBuscado == null) throw new KeyNotFoundException("El producto que se desea eliminar no existe.");            
            await _productoRepository.DeleteAsync(productoBuscado);
        }
    }
}
