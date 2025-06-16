using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;
using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.Services.Miembro;

namespace SuperPrecios.Web.Controllers.Administrador
{    
    public class MarcaController : Controller
    {
        private readonly IMarcaAddService _marcaAddService;
        private readonly IMarcaGetService _marcaGetService;
        private readonly IMarcaDeleteService _marcaDeleteService;

        public MarcaController(IMarcaAddService marcaAddService, IMarcaGetService marcaGetService, IMarcaDeleteService marcaDeleteService)
        {
            _marcaAddService = marcaAddService;
            _marcaGetService = marcaGetService;
            _marcaDeleteService = marcaDeleteService;
        }

        // GET: MarcaController
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var marcas = await _marcaGetService.GetAll();
                if (marcas == null || !marcas.Any()) throw new Exception("No existen marcas");                                    
                return View(marcas);                
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }

        // GET: MarcaController/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var miembroBuscado = await _marcaGetService.GetById(id);
                return View(miembroBuscado);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: MarcaController/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: MarcaController/Create
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DtoMarcaAdd dto)
        {
            try
            {
                await _marcaAddService.AddAsync(dto);
                TempData["Message"] = "Marca agregada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View(dto);
            }
        }

        // GET: MarcaController/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var marca = await _marcaGetService.GetById(id);                
                return View(marca);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }            
        }

        // POST: MarcaController/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DtoMarcaGet dto)
        {
            try
            {
                await _marcaDeleteService.Delete(dto.Id);
                TempData["Message"] = "Marca eliminada correctamente";                
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;                
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
