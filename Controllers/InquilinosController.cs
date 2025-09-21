using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class InquilinosController : Controller
    {
        private readonly InquilinoRepository _repo;
        public InquilinosController(InquilinoRepository repo) => _repo = repo;

        public IActionResult Index() => View(_repo.GetAll());

        public IActionResult Details(int id)
        {
            var i = _repo.GetById(id);
            return i == null ? NotFound() : View(i);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino i)
        {
            if (string.IsNullOrWhiteSpace(i.Dni)) ModelState.AddModelError(nameof(i.Dni), "DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Apellido)) ModelState.AddModelError(nameof(i.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Nombre)) ModelState.AddModelError(nameof(i.Nombre), "Nombre es obligatorio");
            if (_repo.ExistsDni(i.Dni)) ModelState.AddModelError(nameof(i.Dni), "DNI ya registrado");
            if (!ModelState.IsValid) return View(i);

            try
            {
                var id = _repo.Create(i);
                TempData["Ok"] = "Inquilino creado.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(i.Dni), "DNI duplicado (único).");
                return View(i);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(i);
            }
        }

        public IActionResult Edit(int id)
        {
            var i = _repo.GetById(id);
            return i == null ? NotFound() : View(i);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino i)
        {
            if (id != i.Id) return BadRequest();
            if (string.IsNullOrWhiteSpace(i.Dni)) ModelState.AddModelError(nameof(i.Dni), "DNI es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Apellido)) ModelState.AddModelError(nameof(i.Apellido), "Apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(i.Nombre)) ModelState.AddModelError(nameof(i.Nombre), "Nombre es obligatorio");
            if (_repo.ExistsDni(i.Dni, excludeId: id)) ModelState.AddModelError(nameof(i.Dni), "DNI ya registrado en otro");
            if (!ModelState.IsValid) return View(i);

            try
            {
                var rows = _repo.Update(i);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Inquilino actualizado.";
                return RedirectToAction(nameof(Details), new { id = i.Id });
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                ModelState.AddModelError(nameof(i.Dni), "DNI duplicado (único).");
                return View(i);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(i);
            }
        }

        public IActionResult Delete(int id)
        {
            var i = _repo.GetById(id);
            return i == null ? NotFound() : View(i);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Inquilino eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var i = _repo.GetById(id);
                return i == null ? NotFound() : View("Delete", i);
            }
        }
    }
}
