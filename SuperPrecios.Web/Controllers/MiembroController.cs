using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;
using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IServices.Miembro;

namespace MVC.Controllers
{
    public class MiembroController : Controller
    {
        private readonly IMiembroGet _miembroGetService;
        private readonly IMiembroAddService _miembroAddService;
        private readonly IMiembroUpdateService _miembroUpdateService;
        private readonly IMiembroDeleteService _miembroDeleteService;

        public MiembroController(IMiembroGet miembroGetService, IMiembroAddService miembroAddService, IMiembroUpdateService miembroUpdateService, IMiembroDeleteService miembroDeleteService)
        {            
            _miembroGetService = miembroGetService;
            _miembroAddService = miembroAddService;
            _miembroUpdateService = miembroUpdateService;
            _miembroDeleteService = miembroDeleteService;
        }

        // GET: MiembrosController
        [AdminFilter]
        public async Task<IActionResult> Index()
        {             
            try
            {
                var miembros = await _miembroGetService.Run();
                if (miembros == null || miembros.Count() == 0)
                {
                    ViewBag.Message = "No existen miembros";
                    return View();
                }
                else
                {
                    return View(miembros);
                }
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }                        
        }

        // GET: MiembrosController/Details/5
        [AdminFilter]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var miembro = _miembroGetService.Run(id);
                if (id <= 0)
                {
                    TempData["Mensaje"] = "No existe miembros registrados";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var dto = await _miembroGetService.Run(id);
                    if (dto == null)
                    {
                        throw new Exception("No existen miembros con ese id");
                    }
                    else
                    {
                        return View(dto);
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Mensaje"] = $"Error:  {e.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: MiembrosController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MiembrosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DtoMiembroAdd dto)
        {
            try
            {
                await _miembroAddService.Run(dto);
                TempData["Success"] = "Miembro creado correctamente.";                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error");
                return View();
            }
        }

        // GET: MiembrosController/Edit/5
        [AdminFilter]
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: MiembrosController/Edit/5
        [AdminFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MiembrosController/Delete/5
        [AdminFilter]
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: MiembrosController/Delete/5
        [AdminFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
