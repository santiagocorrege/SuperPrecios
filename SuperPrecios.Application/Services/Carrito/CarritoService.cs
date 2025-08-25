using SuperPrecios.Application.DTO.Carrito;
using SuperPrecios.Application.IServices.Carrito;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;

namespace SuperPrecios.Application.Services.Carrito
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _repoCarrito;
        private readonly IProductoRepository _repoProductos;

        public CarritoService(ICarritoRepository repoCarrito, IProductoRepository repoProductos)
        {
            _repoCarrito = repoCarrito ?? throw new ArgumentNullException(nameof(repoCarrito));
            _repoProductos = repoProductos ?? throw new ArgumentNullException(nameof(repoProductos));
        }

        public async Task<DtoCarrito> AgregarLineaAsync(int usuarioId, int productoId, int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad no es válida");

            var carrito = await GetCarritoEntityByUsuarioIdAsync(usuarioId);
            var producto = await _repoProductos.GetByIdAsync(productoId);
            if (producto == null) throw new ArgumentNullException(nameof(productoId), "El producto que desea agregar no existe");

            Linea linea = new Linea(producto, carrito, cantidad);
            carrito.AgregarLinea(linea);
            await _repoCarrito.SaveAsync();

            return MapperCarrito.ToDto(carrito);
        }

        public async Task<DtoCarrito> QuitarLineaAsync(int usuarioId, int productoId)
        {
            var carrito = await GetCarritoEntityByUsuarioIdAsync(usuarioId);
            carrito.QuitarLineaPorProducto(productoId);
            await _repoCarrito.SaveAsync();

            return MapperCarrito.ToDto(carrito);
        }

        public async Task<DtoCarrito> ModificarCantidadLineaAsync(int usuarioId, int productoId, int nuevaCantidad)
        {
            var carrito = await GetCarritoEntityByUsuarioIdAsync(usuarioId);
            carrito.ModificarCantidadLinea(productoId, nuevaCantidad);
            await _repoCarrito.SaveAsync();

            return MapperCarrito.ToDto(carrito);
        }

        public async Task<DtoCarrito> LimpiarCarritoAsync(int usuarioId)
        {
            var carrito = await GetCarritoEntityByUsuarioIdAsync(usuarioId);
            carrito.LimpiarCarrito();
            await _repoCarrito.SaveAsync();

            return MapperCarrito.ToDto(carrito);
        }

        public async Task<DtoCarrito> GetCarritoByUsuarioIdAsync(int usuarioId)
        {
            var carrito = await GetCarritoEntityByUsuarioIdAsync(usuarioId);
            return MapperCarrito.ToDto(carrito);
        }

        public async Task<DtoCarritoMini> GetCarritoMiniByUsuarioIdAsync(int usuarioId)
        {
            var carrito = await GetCarritoEntityByUsuarioIdAsync(usuarioId);
            return MapperCarrito.ToCarritoMini(carrito);
        }
        
        private async Task<Domain.Entities.Carrito> GetCarritoEntityByUsuarioIdAsync(int usuarioId)
        {
            if (usuarioId <= 0) throw new ArgumentException("El id del usuario no es válido", nameof(usuarioId));

            var carrito = await _repoCarrito.GetByUsuarioIdAsync(usuarioId);
            if (carrito == null) throw new KeyNotFoundException("El carrito no está disponible en este momento");

            return carrito;
        }
    }
}