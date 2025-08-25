using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.Carrito;
using SuperPrecios.Application.IServices.Carrito;
using SuperPrecios.Web.Models.Carrito;
using System.Security.Claims;

namespace SuperPrecios.Web.Controllers.Carrito
{
    [Authorize(Roles = "Miembro")]
    [Route("Carrito")]
    public class CarritoController : Controller
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService ?? throw new ArgumentNullException(nameof(carritoService));
        }

        // GET: /Carrito - Vista completa del carrito
        [HttpGet("")]
        public IActionResult Index()
        {
            // La vista se carga desde localStorage, luego se sincroniza con servidor
            return View();
        }

        // POST: /Carrito/Agregar - Devuelve JSON del carrito actualizado desde BD
        [HttpPost("Agregar")]
        public async Task<IActionResult> AgregarProducto(int productoId, int cantidad = 1)
        {
            try
            {
                var usuarioId = GetUsuarioId();

                // SIEMPRE consulta BD, nunca estado local
                var carritoDto = await _carritoService.AgregarLineaAsync(usuarioId, productoId, cantidad);

                // Devolver estado del carrito desde BD
                return Json(new VMCarritoResponse
                {
                    Exito = true,
                    Mensaje = "Producto agregado al carrito",
                    Carrito = VMCarritoJson.FromDto(carritoDto)
                });
            }
            catch (Exception ex)
            {
                return Json(new VMCarritoResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                });
            }
        }

        // POST: /Carrito/ModificarCantidad - Devuelve JSON del carrito actualizado desde BD
        [HttpPost("ModificarCantidad")]
        public async Task<IActionResult> ModificarCantidad(int productoId, int cantidad)
        {
            try
            {
                var usuarioId = GetUsuarioId();

                // SIEMPRE consulta BD
                var carritoDto = await _carritoService.ModificarCantidadLineaAsync(usuarioId, productoId, cantidad);

                return Json(new VMCarritoResponse
                {
                    Exito = true,
                    Mensaje = "Cantidad actualizada",
                    Carrito = VMCarritoJson.FromDto(carritoDto)
                });
            }
            catch (Exception ex)
            {
                return Json(new VMCarritoResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                });
            }
        }

        // POST: /Carrito/Quitar - Devuelve JSON del carrito actualizado desde BD
        [HttpPost("Quitar")]
        public async Task<IActionResult> QuitarLinea(int productoId)
        {
            try
            {
                var usuarioId = GetUsuarioId();

                // SIEMPRE consulta BD
                var carritoDto = await _carritoService.QuitarLineaAsync(usuarioId, productoId);

                return Json(new VMCarritoResponse
                {
                    Exito = true,
                    Mensaje = "Producto eliminado",
                    Carrito = VMCarritoJson.FromDto(carritoDto)
                });
            }
            catch (Exception ex)
            {
                return Json(new VMCarritoResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                });
            }
        }

        // POST: /Carrito/Limpiar - Devuelve JSON del carrito actualizado desde BD
        [HttpPost("Limpiar")]
        public async Task<IActionResult> LimpiarCarrito()
        {
            try
            {
                var usuarioId = GetUsuarioId();

                // SIEMPRE consulta BD
                var carritoDto = await _carritoService.LimpiarCarritoAsync(usuarioId);

                return Json(new VMCarritoResponse
                {
                    Exito = true,
                    Mensaje = "Carrito vaciado",
                    Carrito = VMCarritoJson.FromDto(carritoDto)
                });
            }
            catch (Exception ex)
            {
                return Json(new VMCarritoResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                });
            }
        }

        // GET: /Carrito/Estado - Para sincronizar localStorage con BD
        [HttpGet("Estado")]
        public async Task<IActionResult> GetEstadoCarrito()
        {
            try
            {
                var usuarioId = GetUsuarioId();

                // SIEMPRE consulta BD
                var carritoDto = await _carritoService.GetCarritoByUsuarioIdAsync(usuarioId);

                return Json(new VMCarritoResponse
                {
                    Exito = true,
                    Carrito = VMCarritoJson.FromDto(carritoDto)
                });
            }
            catch (Exception ex)
            {
                return Json(new VMCarritoResponse
                {
                    Exito = false,
                    Mensaje = ex.Message
                });
            }
        }

        private int GetUsuarioId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado correctamente");
            }
            return usuarioId;
        }

        // GET: /Carrito/Contenido - Para cargar el partial HTML con el botón de recomendaciones
        [HttpGet("Contenido")]
        public async Task<IActionResult> GetContenidoCarrito()
        {
            try
            {
                var usuarioId = GetUsuarioId();
                var carritoDto = await _carritoService.GetCarritoByUsuarioIdAsync(usuarioId);

                var viewModel = new VMCarritoCompleto
                {
                    Carrito = carritoDto
                };

                return PartialView("_CarritoContenidoPartial", viewModel);
            }
            catch (Exception ex)
            {
                var viewModelError = new VMCarritoCompleto
                {
                    Carrito = new DtoCarrito(),
                    MensajeInfo = "Error al cargar el carrito: " + ex.Message
                };

                return PartialView("_CarritoContenidoPartial", viewModelError);
            }
        }
    }
}