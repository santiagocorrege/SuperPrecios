using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.IServices.PrecioHistorico;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Web.Models.Producto;

namespace SuperPrecios.Web.Controllers.Miembro
{    
    public class VisitanteController : Controller
    {        
        
        private readonly IProductoGetService _productoGetService;

        public VisitanteController(IProductoGetService productoGetService)
        {
            _productoGetService = productoGetService;
        }


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
