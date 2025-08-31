using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly PropietarioRepository _repo;

        public PropietariosController(PropietarioRepository repo)
        {
            _repo = repo;
        }

        // GET: /Propietarios
        public IActionResult Index()
        {
            var lista = _repo.GetAll();
            return View(lista);
        }

        // GET: /Propietarios/Details/5
        public IActionResult Details(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // GET: /Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario p)
        {
            // Validación mínima sin DataAnnotations
            if (string.IsNullOrWhiteSpace(p.Dni))
                ModelState.AddModelError(nameof(p.Dni), "DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(p.Apellido))
                ModelState.AddModelError(nameof(p.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(p.Nombre))
                ModelState.AddModelError(nameof(p.Nombre), "Nombre es obligatorio");

            if (_repo.ExistsDni(p.Dni))
                ModelState.AddModelError(nameof(p.Dni), "Ya existe un propietario con ese DNI");

            if (!ModelState.IsValid)
                return View(p);

            try
            {
                var newId = _repo.Create(p);
                TempData["Ok"] = "Propietario creado correctamente";
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                ModelState.AddModelError(nameof(p.Dni), "DNI duplicado (unique).");
                return View(p);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear: {ex.Message}");
                return View(p);
            }
        }

        // GET: /Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario p)
        {
            if (id != p.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(p.Dni))
                ModelState.AddModelError(nameof(p.Dni), "DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(p.Apellido))
                ModelState.AddModelError(nameof(p.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(p.Nombre))
                ModelState.AddModelError(nameof(p.Nombre), "Nombre es obligatorio");

            if (_repo.ExistsDni(p.Dni, excludeId: id))
                ModelState.AddModelError(nameof(p.Dni), "Ya existe otro propietario con ese DNI");

            if (!ModelState.IsValid)
                return View(p);

            try
            {
                var rows = _repo.Update(p);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Propietario actualizado";
                return RedirectToAction(nameof(Details), new { id = p.Id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(p.Dni), "DNI duplicado (unique).");
                return View(p);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                return View(p);
            }
        }

        // GET: /Propietarios/Delete/5
        public IActionResult Delete(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Propietario eliminado";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex)
            {
                // Por ejemplo, si la FK de inmueble bloquea la baja:
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var p = _repo.GetById(id);
                if (p == null) return NotFound();
                return View("Delete", p);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar: {ex.Message}");
                var p = _repo.GetById(id);
                if (p == null) return NotFound();
                return View("Delete", p);
            }
        }
    }
}
