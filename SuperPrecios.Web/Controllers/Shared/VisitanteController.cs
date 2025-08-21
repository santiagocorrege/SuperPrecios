using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Web.Models.Producto;

namespace SuperPrecios.Web.Controllers.Shared
{
    [Route("Productos")]
    public class VisitanteController : Controller
    {        
        
        private readonly IProductoGetService _productoGetService;

        public VisitanteController(IProductoGetService productoGetService)
        {
            _productoGetService = productoGetService;
        }


        [HttpGet("")]
        public async Task<IActionResult> Productos(string busqueda = null, int pagina = 1)
        {
            try
            {
                var dtoProductosPaginados = string.IsNullOrWhiteSpace(busqueda)
                    ? await _productoGetService.GetProductosTodayWPrecioHistoricoPaginado(pagina)
                    : await _productoGetService.GetProductosByNameTodayWPrecioHistoricoPaginado(busqueda, pagina);

                VMProductoCompletoWPaginado vm = new VMProductoCompletoWPaginado
                {
                    Productos = dtoProductosPaginados.DtoProductos,
                    PaginaActual = dtoProductosPaginados.PaginaActual,
                    TotalPaginas = dtoProductosPaginados.TotalPaginas,
                };

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_ProductosConPaginadoPartial", vm);
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new VMProductoCompletoWPaginado());
            }        
        }

        // GET /Productos/Categoria/131
        [HttpGet("Categoria/{categoriaId}")]
        public async Task<IActionResult> ProductosPorCategoria(int categoriaId, int pagina = 1)
        {
            try
            {
                var dto = await _productoGetService
                .GetProductosByCategoriaTodayWPrecioHistoricoPaginado(categoriaId, pagina);
                
                var vm = new VMProductoCompletoWPaginado
                {
                    Productos = dto.DtoProductos,
                    PaginaActual = dto.PaginaActual,
                    TotalPaginas = dto.TotalPaginas,
                    Categoria = dto.Categoria
                };

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_ProductosConPaginadoPartial", vm);
                return View("Productos", vm);
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Productos", new VMProductoCompletoWPaginado());
            }                        
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> ProductoDetail(int id)
        {
            //DtoProductoCompleto
            try
            {
                var producto = await _productoGetService.GetCompletoByIdAsync(id);
                return View(producto);
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message; 
                return RedirectToAction(nameof(Productos));
            }
            
        }

    }
}
