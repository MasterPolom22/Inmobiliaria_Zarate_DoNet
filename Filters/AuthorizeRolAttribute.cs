using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Inmobiliaria_Zarate_DoNet.Utils;

namespace Inmobiliaria_Zarate_DoNet.Filters
{
    public class AuthorizeRolAttribute : ActionFilterAttribute
    {
        public string Roles { get; set; } = ""; // "ADMIN" o "ADMIN,EMPLEADO"
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var userId = http.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
            {
                var returnUrl = http.Request.Path + http.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
                return;
            }
            var rol = http.Session.GetString(SessionKeys.UserRol) ?? "";
            if (!string.IsNullOrEmpty(Roles))
            {
                var permitidos = Roles.Split(',').Select(r => r.Trim());
                if (!permitidos.Contains(rol))
                {
                    context.Result = new ForbidResult(); // 403
                    return;
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
