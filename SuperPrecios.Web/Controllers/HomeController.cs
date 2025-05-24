using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.Web.Models;
using SuperPrecios.Web.Models.Usuario;

namespace SuperPrecios.Web.Controllers;

public class HomeController : Controller
{    
    private IUsuarioLoginService _getUsuarioLoginService;

    public HomeController(IUsuarioLoginService getUsuarioLogin)
    {
        _getUsuarioLoginService = getUsuarioLogin;
    }

    public IActionResult Login()
    {
        return View(new UsuarioLoginModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(UsuarioLoginModel model)
    {
        try
        {
            if (model == null || String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Rellene todos los campos";
                return View(model);
            }
            var user = await _getUsuarioLoginService.Run(model.Email, model.Password);
            if (user == null)
            {
                throw new Exception("Usuario y/o password invalidas");
            }
            else
            {                
                string rol = user.Rol;
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Rol", rol);
                if (rol == "Administrador")
                {
                    return RedirectToAction("Index", rol);
                }
                else
                {
                    TempData["Error"] = "Aun no existen acciones para el Miembro";
                    return RedirectToAction("Login", "Home");
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View();
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}
