using System.IO;
using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Inmobiliaria_Zarate_DoNet.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class PerfilController : Controller
    {
        private readonly UsuarioRepository _repo;
        private readonly IWebHostEnvironment _env;
        public PerfilController(UsuarioRepository repo, IWebHostEnvironment env) { _repo = repo; _env = env; }

        // Ruta de avatar por convención
        private string AvatarFilePath(int userId) => Path.Combine(_env.WebRootPath, "avatars", $"{userId}.jpg");
        private string AvatarUrlOrDefault(int userId) => System.IO.File.Exists(AvatarFilePath(userId)) ? $"/avatars/{userId}.jpg" : "/img/avatar-default.png";

        public IActionResult Datos()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Datos([Bind("Id,Nombre,Apellido,Email,Activo")] Usuario u)
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            if (u.Id != id) return Forbid();

            u.Email = (u.Email ?? "").Trim().ToLowerInvariant();
            u.Nombre = (u.Nombre ?? "").Trim();
            u.Apellido = (u.Apellido ?? "").Trim();
            if (string.IsNullOrWhiteSpace(u.Nombre)) ModelState.AddModelError(nameof(u.Nombre), "Nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Apellido)) ModelState.AddModelError(nameof(u.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Email)) ModelState.AddModelError(nameof(u.Email), "Email es obligatorio");
            if (_repo.ExistsEmail(u.Email, excludeId: id)) ModelState.AddModelError(nameof(u.Email), "Email ya registrado en otro");
            if (!ModelState.IsValid) { ViewBag.AvatarUrl = AvatarUrlOrDefault(id); return View(u); }

            var actual = _repo.GetById(id);
            if (actual == null) return NotFound();
            u.Rol = actual.Rol; // no se puede cambiar rol desde perfil

            _repo.Update(u);
            TempData["Ok"] = "Datos actualizados.";
            return RedirectToAction(nameof(Datos));
        }

        public IActionResult Avatar()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Avatar(IFormFile archivo)
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            if (id == 0) return Forbid();
            if (archivo == null || archivo.Length == 0) { ModelState.AddModelError("", "Seleccioná una imagen."); var u = _repo.GetById(id); ViewBag.AvatarUrl = AvatarUrlOrDefault(id); return View(u); }
            if (archivo.Length > 2 * 1024 * 1024) { ModelState.AddModelError("", "Máximo 2 MB."); var u = _repo.GetById(id); ViewBag.AvatarUrl = AvatarUrlOrDefault(id); return View(u); }
            if (archivo.ContentType != "image/jpeg" && archivo.ContentType != "image/png") { ModelState.AddModelError("", "Solo JPG o PNG."); var u = _repo.GetById(id); ViewBag.AvatarUrl = AvatarUrlOrDefault(id); return View(u); }

            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "avatars"));
            using var stream = new FileStream(AvatarFilePath(id), FileMode.Create);
            archivo.CopyTo(stream);

            TempData["Ok"] = "Avatar actualizado.";
            return RedirectToAction(nameof(Avatar));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAvatar()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var path = AvatarFilePath(id);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            TempData["Ok"] = "Avatar eliminado.";
            return RedirectToAction(nameof(Avatar));
        }

        public IActionResult Password() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Password(string Actual, string Nueva, string Repetir)
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();

            if (string.IsNullOrWhiteSpace(Actual)) ModelState.AddModelError(nameof(Actual), "Contraseña actual requerida");
            if (string.IsNullOrWhiteSpace(Nueva)) ModelState.AddModelError(nameof(Nueva), "Nueva contraseña requerida");
            if (Nueva != Repetir) ModelState.AddModelError(nameof(Repetir), "No coinciden");
            if (!ModelState.IsValid) return View();

            if (!BCrypt.Net.BCrypt.Verify(Actual, u.PasswordHash))
            {
                ModelState.AddModelError(nameof(Actual), "Contraseña actual inválida");
                return View();
            }

            _repo.UpdatePassword(id, Nueva);
            TempData["Ok"] = "Contraseña actualizada.";
            return RedirectToAction(nameof(Password));
        }
    }
}
