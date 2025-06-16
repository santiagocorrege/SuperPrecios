using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;
using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.IServices.Miembro;

namespace SuperPrecios.Web.Controllers.Administrador
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {             
            try
            {
                var miembros = await _miembroGetService.Run();
                if (miembros == null || miembros.Count() == 0) throw new Exception ("No existen miembros");
                return View(miembros);                
            }
            catch(Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }                        
        }

        // GET: MiembrosController/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var miembroBuscado = await _miembroGetService.RunById(id);                                                           
                return View(miembroBuscado);                
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        // POST: MiembrosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DtoMiembroAdd dto)
        {
            try
            {
                await _miembroAddService.Run(dto);
                TempData["Message"] = "Miembro creado correctamente.";                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;                
                return View();
            }
        }

        // GET: MiembrosController/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "No existe el miembro seleccionado";
                RedirectToAction(nameof(Index));
            }
            try
            {
                var miembroBuscado = await _miembroGetService.RunGetUpdate(id);
                return View(miembroBuscado);
            }
            catch (Exception e)
            {
                TempData["Message"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MiembrosController/Edit/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DtoMiembroUpdate dto)
        {
            if(dto == null || !ModelState.IsValid)
            {
                TempData["Error"] = "No se pudo actualizar el miembro. Datos inválidos.";                
                return RedirectToAction(nameof(Index));
            }
            try
            {
                await _miembroUpdateService.Run(dto);
                ViewBag.Message = "Miembro actualizado correctamente.";
                dto.Password = string.Empty; // Clear password to avoid showing it in the view
                return View(dto);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                dto.Password = string.Empty; // Clear password to avoid showing it in the view
                return View(dto);
            }
        }

        // GET: MiembrosController/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {            
            try
            {                
                var miembro = await _miembroGetService.RunById(id);
                return View(miembro);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;                
            }
            return RedirectToAction(nameof(Index));
        }
        // POST: MiembrosController/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DtoMiembroGet dto)
        {            
            try
            {
                if (dto == null || dto.Id < 1) throw new ArgumentException("El ID del miembro no es válido.");                                    
                await _miembroDeleteService.Run(dto.Id);
                TempData["Message"] = "Miembro eliminado correctamente.";
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetByNombreMiembros(string emailMiembro)
        {
            try
            {
                IEnumerable<DtoMiembroGet> dtoMiembros = await _miembroGetService.RunByEmailList(emailMiembro);
                if (dtoMiembros == null || dtoMiembros.Count() == 0)
                {
                    throw new Exception("No hay miembros con ese nombre");
                }
                else
                {
                    return View("Index", dtoMiembros);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
