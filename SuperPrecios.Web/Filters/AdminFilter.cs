using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;

            if (controller == null)
            {
                base.OnActionExecuting(context);
                return;
            }

            var session = controller.HttpContext.Session;
            string rol = session.GetString("Rol");

            if (string.IsNullOrEmpty(rol) || rol != "Administrador")
            {
                controller.TempData["Error"] = "Usted no tiene permisos para acceder a esta sección.";
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
