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
        public async Task<ActionResult> Index()
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MiembrosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MiembrosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DtoMiembroAdd dto)
        {
            try
            {
                await _miembroAddService.Run(dto);
                TempData["Success"] = "Miembro creado correctamente.";
                Console.WriteLine("Nice");
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MiembrosController/Edit/5
        [AdminFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MiembrosController/Delete/5
        [AdminFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
