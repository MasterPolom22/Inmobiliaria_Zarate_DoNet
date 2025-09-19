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

        public PerfilController(UsuarioRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        // -------- Helpers de avatar (convención) ------------
        private string AvatarFilePath(int userId)
            => Path.Combine(_env.WebRootPath, "avatars", $"{userId}.jpg");

        private string AvatarUrlOrDefault(int userId)
        {
            var f = AvatarFilePath(userId);
            return System.IO.File.Exists(f) ? $"/avatars/{userId}.jpg" : "/img/avatar-default.png";
        }
        // -----------------------------------------------------

        // GET: /Perfil/Datos
        public IActionResult Datos()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            ViewBag.AvatarUrl = AvatarUrlOrDefault(u.Id);
            return View(u);
        }

        // POST: /Perfil/Datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Datos([Bind("Id,Nombre,Apellido,Email,Activo")] Usuario u)
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            if (u.Id != id) return Forbid(); // no podés editar a otro

            // Normalizaciones
            u.Email = (u.Email ?? "").Trim().ToLowerInvariant();
            u.Nombre = (u.Nombre ?? "").Trim();
            u.Apellido = (u.Apellido ?? "").Trim();

            // Validaciones básicas iguales a Usuarios/Edit (sin rol)
            if (string.IsNullOrWhiteSpace(u.Nombre))
                ModelState.AddModelError(nameof(u.Nombre), "Nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Apellido))
                ModelState.AddModelError(nameof(u.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Email))
                ModelState.AddModelError(nameof(u.Email), "Email es obligatorio");

            // Email único (excluyendo mi propio id)
            if (_repo.ExistsEmail(u.Email, excludeId: id))
                ModelState.AddModelError(nameof(u.Email), "Ya existe otro usuario con ese email");

            if (!ModelState.IsValid)
            {
                ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
                return View(u);
            }

            // No permitimos cambiar el Rol desde el perfil
            var actual = _repo.GetById(id);
            if (actual == null) return NotFound();
            u.Rol = actual.Rol;

            _repo.Update(u); // Update parcial como en tus repos (igual a Usuarios.Update) :contentReference[oaicite:4]{index=4}
            TempData["Ok"] = "Datos actualizados";
            return RedirectToAction(nameof(Datos));
        }

        // GET: /Perfil/Avatar
        public IActionResult Avatar()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
            return View(u);
        }

        // POST: /Perfil/Avatar (subir)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Avatar(IFormFile archivo)
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            if (id == 0) return Forbid();

            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("", "Seleccioná una imagen.");
                ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
                var u = _repo.GetById(id);
                return View(u);
            }

            // Validaciones de imagen (simple): tamaño máx 2MB y content-type imagen
            if (archivo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("", "La imagen no puede superar 2 MB.");
                ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
                var u = _repo.GetById(id);
                return View(u);
            }
            if (archivo.ContentType != "image/jpeg" && archivo.ContentType != "image/png")
            {
                ModelState.AddModelError("", "Solo se permiten JPG o PNG.");
                ViewBag.AvatarUrl = AvatarUrlOrDefault(id);
                var u = _repo.GetById(id);
                return View(u);
            }

            // Guardar como JPG por convención
            Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "avatars"));
            var destino = AvatarFilePath(id);

            using (var stream = new FileStream(destino, FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            TempData["Ok"] = "Avatar actualizado";
            return RedirectToAction(nameof(Avatar));
        }

        // POST: /Perfil/EliminarAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAvatar()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            if (id == 0) return Forbid();

            var path = AvatarFilePath(id);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            TempData["Ok"] = "Avatar eliminado";
            return RedirectToAction(nameof(Avatar));
        }

        // GET: /Perfil/Password
        public IActionResult Password()
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View();
        }

        // POST: /Perfil/Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Password(string Actual, string Nueva, string Repetir)
        {
            var id = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            var u = _repo.GetById(id);
            if (u == null) return NotFound();

            if (string.IsNullOrWhiteSpace(Actual))
                ModelState.AddModelError(nameof(Actual), "Contraseña actual es obligatoria");
            if (string.IsNullOrWhiteSpace(Nueva))
                ModelState.AddModelError(nameof(Nueva), "La nueva contraseña es obligatoria");
            if (Nueva != Repetir)
                ModelState.AddModelError(nameof(Repetir), "Las contraseñas no coinciden");

            if (!ModelState.IsValid) return View();

            if (!BCrypt.Net.BCrypt.Verify(Actual, u.PasswordHash))
            {
                ModelState.AddModelError(nameof(Actual), "La contraseña actual no es válida");
                return View();
            }

            _repo.UpdatePassword(id, Nueva);
            TempData["Ok"] = "Contraseña actualizada";
            return RedirectToAction(nameof(Password));
        }
    }
}
