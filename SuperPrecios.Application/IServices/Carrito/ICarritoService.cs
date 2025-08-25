using SuperPrecios.Application.DTO.Carrito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarritoCore = SuperPrecios.Domain.Entities.Carrito;

namespace SuperPrecios.Application.IServices.Carrito
{
    public interface ICarritoService
    {
        // Operaciones principales del carrito
        Task<DtoCarrito> AgregarLineaAsync(int usuarioId, int productoId, int cantidad);
        Task<DtoCarrito> QuitarLineaAsync(int usuarioId, int productoId);
        Task<DtoCarrito> ModificarCantidadLineaAsync(int usuarioId, int productoId, int nuevaCantidad);
        Task<DtoCarrito> LimpiarCarritoAsync(int usuarioId);
        Task<DtoCarrito> GetCarritoByUsuarioIdAsync(int usuarioId);

        // Método adicional para mini-carrito
        Task<DtoCarritoMini> GetCarritoMiniByUsuarioIdAsync(int usuarioId);
    }
}
