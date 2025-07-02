using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SuperPrecios.Application.DTO;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.AuthenticationCore.Entities;
using SuperPrecios.Web.Models;
using SuperPrecios.Web.Models.Usuario;

namespace SuperPrecios.Web.Controllers.Shared;

public class HomeController : Controller
{    
    private IUsuarioLoginService _getUsuarioLoginService;
    private IMiembroAddService _miembroAddService;

    public HomeController(IUsuarioLoginService getUsuarioLogin, IMiembroAddService miembroAddService)
    {
        _getUsuarioLoginService = getUsuarioLogin;
        _miembroAddService = miembroAddService;
    }
    
    public IActionResult Index()
    {        
        var rol = User.FindFirstValue(ClaimTypes.Role);        
        if (rol == "Administrador")
            return RedirectToAction(nameof(Index), "Miembro");       
        return RedirectToAction("Productos", "Visitante");                
    }
    public IActionResult Login()
    {
        return View(new VMUsuarioLogin());
    }

    [HttpPost]
    public async Task<IActionResult> Login(VMUsuarioLogin model)
    {
        try
        {
            var user = await _getUsuarioLoginService.Run(model.Email, model.Password);
            if (user == null) throw new Exception("Usuario y/o password invalidas");            
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Rol)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe 
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal, props); 
                string rol = user.Rol;
                if(rol == "Administrador") return RedirectToAction(nameof(Index), "Miembro");       
                if(rol == "Miembro") return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(AccessDenied));
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(model);
        }
    }

    [Authorize]
    [HttpPost] 
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        //HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }

    public IActionResult AccessDenied() => View();

}
