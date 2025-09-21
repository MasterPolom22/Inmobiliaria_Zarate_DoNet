using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Net.Mail;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    [AuthorizeRol(Roles = "ADMIN")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioRepository _repo;
        public UsuariosController(UsuarioRepository repo) => _repo = repo;

        private static string NormalizeEmail(string? email) => (email ?? "").Trim().ToLowerInvariant();
        private static bool IsValidEmail(string email) { try { return new MailAddress(email).Address == email; } catch { return false; } }

        public IActionResult Index() => View(_repo.GetAll());

        public IActionResult Details(int id)
        {
            var u = _repo.GetById(id);
            return u == null ? NotFound() : View(u);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Nombre, string Apellido, string Email, string Password, string PasswordRepetir, Rol Rol, bool Activo = true)
        {
            Email = NormalizeEmail(Email);
            Nombre = Nombre?.Trim() ?? "";
            Apellido = Apellido?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(Nombre)) ModelState.AddModelError(nameof(Nombre), "Nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(Apellido)) ModelState.AddModelError(nameof(Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(Email)) ModelState.AddModelError(nameof(Email), "Email es obligatorio");
            else if (!IsValidEmail(Email)) ModelState.AddModelError(nameof(Email), "Formato de email inválido");
            if (string.IsNullOrWhiteSpace(Password)) ModelState.AddModelError(nameof(Password), "Contraseña es obligatoria");
            if (Password != PasswordRepetir) ModelState.AddModelError(nameof(PasswordRepetir), "Las contraseñas no coinciden");
            if (_repo.ExistsEmail(Email)) ModelState.AddModelError(nameof(Email), "Email ya registrado");
            if (!ModelState.IsValid) return View();

            try
            {
                var u = new Usuario { Nombre = Nombre, Apellido = Apellido, Email = Email, Rol = Rol, Activo = Activo };
                var id = _repo.Create(u, Password);
                TempData["Ok"] = "Usuario creado.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(Email), "Email duplicado (único).");
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View();
            }
        }

        public IActionResult Edit(int id)
        {
            var u = _repo.GetById(id);
            return u == null ? NotFound() : View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nombre,Apellido,Email,Rol,Activo")] Usuario u)
        {
            if (id != u.Id) return BadRequest();
            u.Email = NormalizeEmail(u.Email);
            u.Nombre = u.Nombre?.Trim() ?? "";
            u.Apellido = u.Apellido?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(u.Nombre)) ModelState.AddModelError(nameof(u.Nombre), "Nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Apellido)) ModelState.AddModelError(nameof(u.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Email)) ModelState.AddModelError(nameof(u.Email), "Email es obligatorio");
            else if (!IsValidEmail(u.Email)) ModelState.AddModelError(nameof(u.Email), "Formato de email inválido");
            if (_repo.ExistsEmail(u.Email, excludeId: id)) ModelState.AddModelError(nameof(u.Email), "Email ya registrado en otro");
            if (!ModelState.IsValid) return View(u);

            try
            {
                var rows = _repo.Update(u);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Usuario actualizado.";
                return RedirectToAction(nameof(Details), new { id = u.Id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(u.Email), "Email duplicado (único).");
                return View(u);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(u);
            }
        }

        public IActionResult Delete(int id)
        {
            var u = _repo.GetById(id);
            return u == null ? NotFound() : View(u);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Usuario eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var u = _repo.GetById(id);
                return u == null ? NotFound() : View("Delete", u);
            }
        }
    }
}
