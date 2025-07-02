using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Domain.TAD;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;
using CategoriaCore = SuperPrecios.Domain.Entities.Categoria;
using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;

namespace SuperPrecios.Application.Services.Producto
{
    public class ProductoGetService : IProductoGetService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaGetService _categoriaGetService;

        public ProductoGetService(IProductoRepository productoRepository, ICategoriaGetService categoriaGetService) 
        {
            _productoRepository = productoRepository;
            _categoriaGetService = categoriaGetService;
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
            var productosPagedResult = await _productoRepository.GetProductosByNombreTodayWPrecioHistorico(nombre, pagina, 10);
            var productos = productosPagedResult.Items;
            if (productos == null || !productos.Any()) throw new ProductoException("No existen precios del producto actualizados para hoy");
            return new DtoProductosPaginados
            {
                DtoProductos = MapperProducto.ToDtoProductoCompleto(productos),
                PaginaActual = productosPagedResult.PaginaActual,
                TotalPaginas = productosPagedResult.TotalPaginas,
            };
        }

        public async Task<DtoProductosPaginados> GetProductosTodayWPrecioHistoricoPaginado(int pagina)
        {
            if (pagina < 1) throw new ProductoException("La pagina seleccionada no es valida");
            var productosPagedResult = await _productoRepository.GetProductosTodayWPrecioHistorico(pagina, 10);
            var productos = productosPagedResult.Items;
            if (productos == null || !productos.Any()) throw new ProductoException("No existen precios actualizados para hoy");
            return new DtoProductosPaginados
            {
                DtoProductos = MapperProducto.ToDtoProductoCompleto(productos),
                PaginaActual = productosPagedResult.PaginaActual,
                TotalPaginas = productosPagedResult.TotalPaginas,                
            };            
        }

        public async Task<DtoProductosPaginados> GetProductosByCategoriaTodayWPrecioHistoricoPaginado(int categoriaId, int pagina)
        {
            if (pagina < 1)
                throw new ProductoException("La página seleccionada no es válida");
            var categoria = await _categoriaGetService.GetById(categoriaId);
            if (categoria == null)
                throw new ProductoException("La categoria seleccionada no existe");
            var categoriaIds = await _categoriaGetService.GetDescendantCategoryIdsAsync(categoriaId);
            var productosPagedResult = await _productoRepository.GetByCategoriasWithPrecioHistoricoAsync(categoriaIds, pagina, 10);
            var productos = productosPagedResult.Items;
            if (!productos.Any())
                throw new ProductoException("No existen precios actualizados para hoy en esa categoría");

            return new DtoProductosPaginados
            {
                DtoProductos = MapperProducto.ToDtoProductoCompleto(productos),
                PaginaActual = productosPagedResult.PaginaActual,
                TotalPaginas = productosPagedResult.TotalPaginas,
                Categoria = categoria.Nombre
            };
        }
    }
}
