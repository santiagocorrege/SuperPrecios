using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;
using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;

namespace SuperPrecios.Web.Controllers.Administrador
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaAddService _categoriaAddService;
        private readonly ICategoriaGetService _categoriaGetService;
        private readonly ICategoriaDeleteService _categoriaDeleteService;

        public CategoriaController(
            ICategoriaAddService categoriaAddService,
            ICategoriaGetService categoriaGetService,
            ICategoriaDeleteService categoriaDeleteService)
        {
            _categoriaAddService = categoriaAddService;
            _categoriaGetService = categoriaGetService;
            _categoriaDeleteService = categoriaDeleteService;
        }

        // GET: CategoriaController
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categorias = await _categoriaGetService.GetAll();
                if (categorias == null || !categorias.Any()) throw new Exception("No existen categorías registradas.");
                return View(categorias);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }

        // GET: CategoriaController/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var categoria = await _categoriaGetService.GetById(id);
                return View(categoria);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CategoriaController/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaController/Create
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DtoCategoriaAdd dto)
        {
            try
            {
                await _categoriaAddService.Run(dto);
                TempData["Message"] = "Categoría agregada correctamente";
                return RedirectToAction(nameof(Create));
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View(dto);
            }
        }

        // GET: CategoriaController/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _categoriaGetService.GetById(id);
                return View(categoria);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }            
        }

        // POST: CategoriaController/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DtoCategoriaGet dto)
        {
            try
            {
                await _categoriaDeleteService.Run(dto.Id);
                TempData["Message"] = "Categoría eliminada correctamente";                
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;                
            }
            return RedirectToAction(nameof(Index));
        }
    }
}