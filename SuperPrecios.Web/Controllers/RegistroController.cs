using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.DTO.Proveedor;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.IServices.Proveedor;

namespace SuperPrecios.Web.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IMiembroAddService _miembroAddService;
        private readonly IProveedorAddService _proveedorAddService;

        public RegistroController(IMiembroAddService miembroAddService, IProveedorAddService proveedorAddService)
        {
            _miembroAddService = miembroAddService;
            _proveedorAddService = proveedorAddService;
        }
        public ActionResult Miembro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Miembro(DtoMiembroAdd dto)
        {
            try
            {
                await _miembroAddService.Run(dto);
                TempData["Message"] = "Miembro creado correctamente.";
                return RedirectToAction(nameof(Login), "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult Proveedor()
        {
                return View();
        }

        // POST: RegistroController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proveedor(DtoProveedorAdd dto)
        {
            try
            {
                await _proveedorAddService.Run(dto);
                TempData["Message"] = "Proveedor creado correctamente.";
                return RedirectToAction(nameof(Login), "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}
