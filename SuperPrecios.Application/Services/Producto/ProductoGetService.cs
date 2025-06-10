using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.IRepositories;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;

namespace SuperPrecios.Application.Services.Producto
{
    public class ProductoGetService : IProductoGetService
    {
        private readonly IProductoRepository _productoRepository;
        public ProductoGetService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }
        public async Task<IEnumerable<DtoProductoGet>> GetAllAsync()
        {
            var productos = await _productoRepository.GetAllAsync();
            return MapperProducto.ToDtoCompletoList(productos);
        }

        public async Task<DtoProductoGet> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El id del producto no es valido");
            ProductoCore productoBuscado = await _productoRepository.GetByIdAsync(id);
            return MapperProducto.ToDtoCompleto(productoBuscado);
        }

        public async Task<DtoProductoGet> GetByNombreAsync(string nombre)
        {
            if (String.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre del producto no es valido");
            ProductoCore productoBuscado = await _productoRepository.GetByNombreAsync(nombre);
            return MapperProducto.ToDtoCompleto(productoBuscado);
        }
    }
}
