using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Utils;


namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioRepository _repo;
        public AuthController(UsuarioRepository repo) => _repo = repo;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, string? returnUrl = null)
        {
            email = (email ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email)) ModelState.AddModelError(nameof(email), "Email es obligatorio");
            if (string.IsNullOrWhiteSpace(password)) ModelState.AddModelError(nameof(password), "Contraseña es obligatoria");
            if (!ModelState.IsValid) return View();

            var u = _repo.GetByEmail(email);
            if (u == null || !u.Activo || !BCrypt.Net.BCrypt.Verify(password, u.PasswordHash))
            {
                ModelState.AddModelError("", "Credenciales inválidas o usuario inactivo.");
                return View();
            }

            // Sesión
            HttpContext.Session.SetInt32(SessionKeys.UserId, u.Id);
            HttpContext.Session.SetString(SessionKeys.UserEmail, u.Email);
            HttpContext.Session.SetString(SessionKeys.UserRol, u.Rol.ToString());
            HttpContext.Session.SetString(SessionKeys.UserNombre, $"{u.Nombre} {u.Apellido}");

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
