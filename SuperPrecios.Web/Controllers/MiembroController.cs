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
                var miembro = await _miembroGetService.Run(id);
                if (id <= 0)
                {
                    TempData["Error"] = "No existe el miembro seleccionado";
                    return RedirectToAction(nameof(Index));
                }
                
                var dto = await _miembroGetService.Run(id);
                if (dto == null)
                {
                    throw new Exception("No existen miembros con ese id");
                }                
                return View(dto);
                
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
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
        [AdminFilter]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "No existe el miembro seleccionado";
                RedirectToAction(nameof(Index));
            }
            try
            {
                var dto = await _miembroGetService.RunGetUpdate(id);
                if (dto == null)
                {
                    throw new Exception("No existen miembros con ese id");
                }
                else
                {
                    return View(dto);
                }
            }
            catch (Exception e)
            {
                TempData["Message"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MiembrosController/Edit/5
        [AdminFilter]
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
            catch
            {
                ViewBag.Error = "Error al actualizar el miembro. Por favor, intente nuevamente.";
                dto.Password = string.Empty; // Clear password to avoid showing it in the view
                return View(dto);
            }
        }

        // GET: MiembrosController/Delete/5
        [AdminFilter]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID inválido.";
                return RedirectToAction(nameof(Index));
            }

            var miembro = await _miembroGetService.Run(id);
            if (miembro == null)
            {
                TempData["Error"] = "Miembro no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(miembro); // Vista con detalles del miembro y un formulario de confirmación
        }
        // POST: MiembrosController/Delete/5
        [AdminFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DtoMiembroGet dto)
        {
            if(dto == null || dto.Id <= 0)
            {
                TempData["Error"] = "No se pudo actualizar el miembro. Datos inválidos.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                await _miembroDeleteService.Run(dto.Id);
                TempData["Message"] = "Miembro eliminado correctamente.";
            }
            catch
            {
                TempData["Error"] = "Error al eliminar el miembro.";
            }

            return RedirectToAction(nameof(Index));
        }

        [AdminFilter]
        public async Task<IActionResult> GetByNombreMiembros(string emailMiembro)
        {
            try
            {
                IEnumerable<DtoMiembroGet> dtoMiembros = await _miembroGetService.RunByNombreList(emailMiembro);
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
