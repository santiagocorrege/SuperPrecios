using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;
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
            return MapperProducto.ToDtoProductoGet(productos);
        }

        public async Task<DtoProductoGet> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El id del producto no es valido");
            ProductoCore productoBuscado = await _productoRepository.GetByIdAsync(id);
            if (productoBuscado == null) throw new ProductoException("No se encontro el producto con el id especificado");
            return MapperProducto.ToDtoCompleto(productoBuscado);
        }

        public async Task<DtoProductoGet> GetByNombreAsync(string nombre)
        {
            if (String.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre del producto no es valido");
            ProductoCore productoBuscado = await _productoRepository.GetByNombreAsync(nombre);
            if (productoBuscado == null) throw new ProductoException("No se encontro el producto con el nombre especificado");
            return MapperProducto.ToDtoCompleto(productoBuscado);
        }

        public async Task<DtoProductoCompleto> GetCompletoByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("El id del producto no es valido");
            ProductoCore productoBuscado = await _productoRepository.GetProductoTodayWPrecioHistorico(id);
            if (productoBuscado == null) throw new ProductoException("No se encontro el producto con el id especificado");
            return MapperProducto.ToDtoProductoCompleto(productoBuscado);
        }
        
        public async Task<DtoProductosPaginados> GetProductosByNameTodayWPrecioHistoricoPaginado(string nombre, int pagina)
        {
            if (nombre == null || String.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre del producto no es valido");
            if (pagina < 1) throw new ProductoException("La pagina seleccionada no es valida");
            var dtoProductoPaginado = await _productoRepository.GetProductosByNombreTodayWPrecioHistorico(nombre, pagina);
            var productos = dtoProductoPaginado.Productos;
            if (productos == null || !productos.Any()) throw new ProductoException("No existen precios del producto actualizados para hoy");
            return new DtoProductosPaginados
            {
                DtoProductos = MapperProducto.ToDtoProductoCompleto(productos),
                PaginaActual = dtoProductoPaginado.PaginaActual,
                TotalPaginas = dtoProductoPaginado.TotalPaginas,
            };
        }

        public async Task<DtoProductosPaginados> GetProductosTodayWPrecioHistoricoPaginado(int pagina)
        {
            if (pagina < 1) throw new ProductoException("La pagina seleccionada no es valida");
            var dtoProductoPaginado = await _productoRepository.GetProductosTodayWPrecioHistorico(pagina);
            var productos = dtoProductoPaginado.Productos;
            if (productos == null || !productos.Any()) throw new ProductoException("No existen precios actualizados para hoy");
            return new DtoProductosPaginados
            {
                DtoProductos = MapperProducto.ToDtoProductoCompleto(productos),
                PaginaActual = dtoProductoPaginado.PaginaActual,
                TotalPaginas = dtoProductoPaginado.TotalPaginas,                
            };
            
        }
    }
}
