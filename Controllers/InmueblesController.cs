using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using Inmobiliaria_Zarate_DoNet.Filters;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class InmueblesController : Controller
    {
        private readonly InmuebleRepository _repo;

        public InmueblesController(InmuebleRepository repo) => _repo = repo;

        // ====== Helpers para combos ======
        private void CargarCombos(int? propietarioIdSel = null, int? tipoIdSel = null, UsoInmueble? usoSel = null)
        {
            var props = _repo.GetPropietariosForSelect()
                             .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NombreCompleto })
                             .ToList();
            ViewBag.Propietarios = new SelectList(props, "Value", "Text", propietarioIdSel?.ToString());

            var tipos = _repo.GetTiposForSelect()
                             .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre })
                             .ToList();
            ViewBag.Tipos = new SelectList(tipos, "Value", "Text", tipoIdSel?.ToString());

            var usos = new List<SelectListItem>
            {
                new SelectListItem("RESIDENCIAL", UsoInmueble.RESIDENCIAL.ToString()),
                new SelectListItem("COMERCIAL", UsoInmueble.COMERCIAL.ToString())
            };
            var usoSelStr = (usoSel ?? UsoInmueble.RESIDENCIAL).ToString();
            ViewBag.Usos = new SelectList(usos, "Value", "Text", usoSelStr);
        }

        // ====== INDEX ======
        public IActionResult Index()
        {
            var lista = _repo.GetAll();
            return View(lista);
        }

        // ====== DETAILS
        public IActionResult Details(int id)
        {
            var x = _repo.GetById(id);
            if (x == null) return NotFound();
            return View(x);
        }

        // ====== CREATE GET
        public IActionResult Create()
        {
            CargarCombos();
            return View(new Inmueble { Uso = UsoInmueble.RESIDENCIAL, Disponible = true, Suspendido = false });
        }

        // ====== CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble x)
        {
            // Validaciones mínimas
            if (x.PropietarioId <= 0) ModelState.AddModelError(nameof(x.PropietarioId), "Seleccione un propietario.");
            if (x.TipoId <= 0) ModelState.AddModelError(nameof(x.TipoId), "Seleccione un tipo.");
            if (string.IsNullOrWhiteSpace(x.Direccion)) ModelState.AddModelError(nameof(x.Direccion), "La dirección es obligatoria.");
            if (x.Ambientes < 1) ModelState.AddModelError(nameof(x.Ambientes), "Ambientes debe ser >= 1.");
            if (x.PrecioBase < 0) ModelState.AddModelError(nameof(x.PrecioBase), "Precio base inválido.");

            if (!ModelState.IsValid)
            {
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }

            try
            {
                var id = _repo.Create(x);
                TempData["Ok"] = "Inmueble creado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"Error MySQL: {ex.Message}");
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }
        }

        // ====== EDIT GET 
        public IActionResult Edit(int id)
        {
            var x = _repo.GetById(id);
            if (x == null) return NotFound();
            CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
            return View(x);
        }

        // ====== EDIT POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble x)
        {
            if (id != x.Id) return BadRequest();

            if (x.PropietarioId <= 0) ModelState.AddModelError(nameof(x.PropietarioId), "Seleccione un propietario.");
            if (x.TipoId <= 0) ModelState.AddModelError(nameof(x.TipoId), "Seleccione un tipo.");
            if (string.IsNullOrWhiteSpace(x.Direccion)) ModelState.AddModelError(nameof(x.Direccion), "La dirección es obligatoria.");
            if (x.Ambientes < 1) ModelState.AddModelError(nameof(x.Ambientes), "Ambientes debe ser >= 1.");
            if (x.PrecioBase < 0) ModelState.AddModelError(nameof(x.PrecioBase), "Precio base inválido.");

            if (!ModelState.IsValid)
            {
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }

            try
            {
                var rows = _repo.Update(x);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Inmueble actualizado.";
                return RedirectToAction(nameof(Details), new { id = x.Id });
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"Error MySQL: {ex.Message}");
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }
        }

        // ====== DELETE GET 
        [AuthorizeRol(Roles="ADMIN")]
        public IActionResult Delete(int id)
        {
            var x = _repo.GetById(id);
            if (x == null) return NotFound();
            return View(x);
        }

        // ====== DELETE POST ======
        [AuthorizeRol(Roles="ADMIN")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Inmueble eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var x = _repo.GetById(id);
                if (x == null) return NotFound();
                return View("Delete", x);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                var x = _repo.GetById(id);
                if (x == null) return NotFound();
                return View("Delete", x);
            }
        }
    }
}
