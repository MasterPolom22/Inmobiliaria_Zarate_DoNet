using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly InquilinoRepository _repo;

        public InquilinosController(InquilinoRepository repo)
        {
            _repo = repo;
        }

        // GET: /Inquilinos
        public IActionResult Index()
        {
            var lista = _repo.GetAll();
            return View(lista);
        }

        // GET: /Inquilinos/Details/5
        public IActionResult Details(int id)
        {
            var i = _repo.GetById(id);
            if (i == null) return NotFound();
            return View(i);
        }

        // GET: /Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino i)
        {
            if (string.IsNullOrWhiteSpace(i.Dni))
                ModelState.AddModelError(nameof(i.Dni), "DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Apellido))
                ModelState.AddModelError(nameof(i.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Nombre))
                ModelState.AddModelError(nameof(i.Nombre), "Nombre es obligatorio");

            if (_repo.ExistsDni(i.Dni))
                ModelState.AddModelError(nameof(i.Dni), "Ya existe un inquilino con ese DNI");

            if (!ModelState.IsValid)
                return View(i);

            try
            {
                var newId = _repo.Create(i);
                TempData["Ok"] = "Inquilino creado correctamente";
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(i.Dni), "DNI duplicado (unique).");
                return View(i);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear: {ex.Message}");
                return View(i);
            }
        }

        // GET: /Inquilinos/Edit/5
        public IActionResult Edit(int id)
        {
            var i = _repo.GetById(id);
            if (i == null) return NotFound();
            return View(i);
        }

        // POST: /Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino i)
        {
            if (id != i.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(i.Dni))
                ModelState.AddModelError(nameof(i.Dni), "DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Apellido))
                ModelState.AddModelError(nameof(i.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Nombre))
                ModelState.AddModelError(nameof(i.Nombre), "Nombre es obligatorio");

            if (_repo.ExistsDni(i.Dni, excludeId: id))
                ModelState.AddModelError(nameof(i.Dni), "Ya existe otro inquilino con ese DNI");

            if (!ModelState.IsValid)
                return View(i);

            try
            {
                var rows = _repo.Update(i);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Inquilino actualizado";
                return RedirectToAction(nameof(Details), new { id = i.Id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(i.Dni), "DNI duplicado (unique).");
                return View(i);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                return View(i);
            }
        }

        // GET: /Inquilinos/Delete/5
        public IActionResult Delete(int id)
        {
            var i = _repo.GetById(id);
            if (i == null) return NotFound();
            return View(i);
        }

        // POST: /Inquilinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Inquilino eliminado";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var i = _repo.GetById(id);
                if (i == null) return NotFound();
                return View("Delete", i);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar: {ex.Message}");
                var i = _repo.GetById(id);
                if (i == null) return NotFound();
                return View("Delete", i);
            }
        }
    }
}
