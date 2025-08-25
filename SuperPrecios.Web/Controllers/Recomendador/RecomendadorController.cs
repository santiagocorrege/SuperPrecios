using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.IServices.Recomendador;
using SuperPrecios.Web.Models.Recomendador;
using System.Security.Claims;

namespace SuperPrecios.Web.Controllers.Recomendador
{
    [Authorize(Roles = "Miembro")]
    [Route("Recomendaciones")]
    public class RecomendadorController : Controller
    {
        private readonly IRecomendadorService _recomendadorService;

        public RecomendadorController(IRecomendadorService recomendadorService)
        {
            _recomendadorService = recomendadorService ?? throw new ArgumentNullException(nameof(recomendadorService));
        }

        // GET: /Recomendaciones - Vista principal de recomendaciones
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarioId = GetUsuarioId();
                var dtoRecomendacion = await _recomendadorService.GetRecomendacionCarrito(usuarioId);

                var viewModel = VMRecomendacion.FromDto(dtoRecomendacion);

                return View(viewModel);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("carrito"))
            {
                TempData["Error"] = "Tu carrito está vacío. Agrega algunos productos para obtener recomendaciones.";
                return RedirectToAction("Index", "Carrito");
            }
            catch (Exception ex) when (ex.Message.Contains("supermercados"))
            {
                TempData["Error"] = "No encontramos supermercados que tengan todos los productos de tu carrito disponibles hoy.";
                return RedirectToAction("Index", "Carrito");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al generar las recomendaciones. Por favor, intenta nuevamente.";
                return RedirectToAction("Index", "Carrito");
            }
        }

        // GET: /Recomendaciones/Json - Para AJAX (futuro uso)
        [HttpGet("Json")]
        public async Task<IActionResult> GetRecomendacionJson()
        {
            try
            {
                var usuarioId = GetUsuarioId();
                var dtoRecomendacion = await _recomendadorService.GetRecomendacionCarrito(usuarioId);

                var viewModel = VMRecomendacion.FromDto(dtoRecomendacion);

                return Json(new
                {
                    success = true,
                    data = viewModel
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
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
    }
}