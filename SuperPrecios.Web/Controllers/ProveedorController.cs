using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.IServices.Proveedor;
using System.Threading.Tasks;
using System.Linq;
using SuperPrecios.Application.DTO.Proveedor;
using MVC.Filters;
using Microsoft.AspNetCore.Authorization;

namespace SuperPrecios.Web.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IProveedorAddService _proveedorAddService;
        private readonly IProveedorUpdateService _proveedorUpdateService;
        private readonly IProveedorDeleteService _proveedorDeleteService;
        private readonly IProveedorGetService _proveedorGetService;

        public ProveedorController(
            IProveedorAddService proveedorAddService,
            IProveedorUpdateService proveedorUpdateService,
            IProveedorDeleteService proveedorDeleteService,
            IProveedorGetService proveedorGetService)
        {
            _proveedorAddService = proveedorAddService;
            _proveedorUpdateService = proveedorUpdateService;
            _proveedorDeleteService = proveedorDeleteService;
            _proveedorGetService = proveedorGetService;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var proveedores = await _proveedorGetService.Run();
                if (proveedores == null || !proveedores.Any())
                    throw new Exception("No hay proveedores registrados.");
                return View(proveedores);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var proveedor = await _proveedorGetService.RunById(id);
                return View(proveedor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DtoProveedorAdd dto)
        {
            try
            {
                await _proveedorAddService.Run(dto);
                TempData["Message"] = "Proveedor creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var proveedor = await _proveedorGetService.RunByIdUpdate(id);
                return View(proveedor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DtoProveedorUpdate dto)
        {
            if (dto == null || !ModelState.IsValid)
            {
                TempData["Error"] = "No se pudo actualizar el proveedor. Datos inválidos.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                await _proveedorUpdateService.Run(dto);
                TempData["Message"] = "Proveedor actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(dto);
            }
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) throw new ArgumentException("Id inválido");
                var proveedor = await _proveedorGetService.RunById(id);
                return View(proveedor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DtoProveedorGet dto)
        {
            try
            {
                if (dto == null || dto.Id < 1)
                    throw new ArgumentException("El ID del proveedor no es válido.");
                await _proveedorDeleteService.Run(dto.Id);
                TempData["Message"] = "Proveedor eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
