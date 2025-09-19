using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Inmobiliaria_Zarate_DoNet.Utils;

namespace Inmobiliaria_Zarate_DoNet.Filters
{
    public class AuthorizeLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var userId = http.Session.GetInt32(SessionKeys.UserId);
            if (userId == null)
            {
                // redirige a Login manteniendo returnUrl
                var returnUrl = http.Request.Path + http.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
            }
            base.OnActionExecuting(context);
        }
    }
}
