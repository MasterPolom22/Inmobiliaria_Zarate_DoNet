using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Net.Mail;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    // [Authorize(Roles="ADMIN")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioRepository _repo;

        public UsuariosController(UsuarioRepository repo)
        {
            _repo = repo;
        }

        // -------- Helpers ----------
        private static string NormalizeEmail(string? email) =>
            (email ?? string.Empty).Trim().ToLowerInvariant();

        private static bool IsValidEmail(string email)
        {
            try { return new MailAddress(email).Address == email; }
            catch { return false; }
        }
        // ----------------------------

        // GET: /Usuarios
        public IActionResult Index()
        {
            var lista = _repo.GetAll();
            return View(lista);
        }

        // GET: /Usuarios/Details/5
        public IActionResult Details(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // GET: /Usuarios/Create
        public IActionResult Create() => View();

        // POST: /Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Nombre, string Apellido, string Email, string Password, string PasswordRepetir, Rol Rol, bool Activo = true)
        {
            // Normalizaciones
            Email = NormalizeEmail(Email);
            Nombre = Nombre?.Trim() ?? "";
            Apellido = Apellido?.Trim() ?? "";

            // Validaciones básicas (como otros módulos)
            if (string.IsNullOrWhiteSpace(Nombre)) ModelState.AddModelError(nameof(Nombre), "Nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(Apellido)) ModelState.AddModelError(nameof(Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(Email)) ModelState.AddModelError(nameof(Email), "Email es obligatorio");
            else if (!IsValidEmail(Email)) ModelState.AddModelError(nameof(Email), "Email no tiene un formato válido");

            // Para crear necesitamos setear el hash una sola vez
            if (string.IsNullOrWhiteSpace(Password)) ModelState.AddModelError(nameof(Password), "Contraseña es obligatoria");
            if (Password != PasswordRepetir) ModelState.AddModelError(nameof(PasswordRepetir), "Las contraseñas no coinciden");

            if (_repo.ExistsEmail(Email))
                ModelState.AddModelError(nameof(Email), "Ya existe un usuario con ese email");

            if (!ModelState.IsValid) return View();

            try
            {
                var u = new Usuario
                {
                    Nombre = Nombre,
                    Apellido = Apellido,
                    Email = Email,
                    Rol = Rol,
                    Activo = Activo
                };

                var newId = _repo.Create(u, Password); // el repo hashea con BCrypt.Net-Next
                TempData["Ok"] = "Usuario creado correctamente";
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (MySqlException ex) when (ex.Number == 1062) // UNIQUE(email)
            {
                ModelState.AddModelError(nameof(Email), "Email duplicado (único).");
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear: {ex.Message}");
                return View();
            }
        }

        // GET: /Usuarios/Edit/5
        public IActionResult Edit(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // POST: /Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nombre,Apellido,Email,Rol,Activo")] Usuario u)
        {
            if (id != u.Id) return BadRequest();

            u.Email = NormalizeEmail(u.Email);
            u.Nombre = u.Nombre?.Trim() ?? "";
            u.Apellido = u.Apellido?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(u.Nombre))
                ModelState.AddModelError(nameof(u.Nombre), "Nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Apellido))
                ModelState.AddModelError(nameof(u.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(u.Email))
                ModelState.AddModelError(nameof(u.Email), "Email es obligatorio");
            else if (!IsValidEmail(u.Email))
                ModelState.AddModelError(nameof(u.Email), "Email no tiene un formato válido");

            // Edit NO toca password_hash (igual que otros módulos: campos visibles nomás)
            if (_repo.ExistsEmail(u.Email, excludeId: id))
                ModelState.AddModelError(nameof(u.Email), "Ya existe otro usuario con ese email");

            if (!ModelState.IsValid) return View(u);

            try
            {
                var rows = _repo.Update(u);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Usuario actualizado";
                return RedirectToAction(nameof(Details), new { id = u.Id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(u.Email), "Email duplicado (único).");
                return View(u);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                return View(u);
            }
        }

        // GET: /Usuarios/Delete/5
        public IActionResult Delete(int id)
        {
            var u = _repo.GetById(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // POST: /Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Usuario eliminado";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var u = _repo.GetById(id);
                if (u == null) return NotFound();
                return View("Delete", u);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar: {ex.Message}");
                var u = _repo.GetById(id);
                if (u == null) return NotFound();
                return View("Delete", u);
            }
        }
    }
}
