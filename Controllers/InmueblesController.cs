using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Linq;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class InmueblesController : Controller
    {
        private readonly InmuebleRepository _repo;
        public InmueblesController(InmuebleRepository repo) => _repo = repo;

        // Combos
        private void CargarCombos(int? propietarioId = null, int? tipoId = null, UsoInmueble? uso = null)
        {
            var props = _repo.GetPropietariosForSelect()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.NombreCompleto }).ToList();
            var tipos = _repo.GetTiposForSelect()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Nombre }).ToList();

            ViewBag.Propietarios = new SelectList(props, "Value", "Text", propietarioId?.ToString());
            ViewBag.Tipos = new SelectList(tipos, "Value", "Text", tipoId?.ToString());

            var usos = new List<SelectListItem> {
                new SelectListItem("RESIDENCIAL", UsoInmueble.RESIDENCIAL.ToString()),
                new SelectListItem("COMERCIAL", UsoInmueble.COMERCIAL.ToString())
            };
            ViewBag.Usos = new SelectList(usos, "Value", "Text", (uso ?? UsoInmueble.RESIDENCIAL).ToString());
        }

        // GET: /Inmuebles
        public IActionResult Index() => View(_repo.GetAll());

        // GET: /Inmuebles/Details/5
        public IActionResult Details(int id)
        {
            var x = _repo.GetById(id);
            return x == null ? NotFound() : View(x);
        }

        // GET: /Inmuebles/Disponibles
        public IActionResult Disponibles()
        {
            var lista = _repo.GetDisponibles();
            return View(lista);
        }

        // GET: /Inmuebles/Libres?inicio=yyyy-MM-dd&fin=yyyy-MM-dd
        public IActionResult Libres(DateTime? inicio, DateTime? fin)
        {
            if (inicio == null || fin == null || fin < inicio)
            {
                // mostrar form para ingresar fechas
                ViewBag.Resultados = null;
                return View();
            }

            var lista = _repo.GetNoOcupadosEntre(inicio.Value, fin.Value);
            ViewBag.Inicio = inicio.Value.ToString("yyyy-MM-dd");
            ViewBag.Fin = fin.Value.ToString("yyyy-MM-dd");
            return View(lista);
        }

        // GET: /Inmuebles/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View(new Inmueble { Uso = UsoInmueble.RESIDENCIAL, Disponible = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble x)
        {
            if (x.PropietarioId <= 0) ModelState.AddModelError(nameof(x.PropietarioId), "Seleccione un propietario.");
            if (x.TipoId <= 0) ModelState.AddModelError(nameof(x.TipoId), "Seleccione un tipo.");
            if (string.IsNullOrWhiteSpace(x.Direccion)) ModelState.AddModelError(nameof(x.Direccion), "Dirección obligatoria.");
            if (x.Ambientes < 1) ModelState.AddModelError(nameof(x.Ambientes), "Ambientes debe ser ≥ 1.");
            if (x.PrecioBase < 0) ModelState.AddModelError(nameof(x.PrecioBase), "Precio inválido.");
            if (!ModelState.IsValid) { CargarCombos(x.PropietarioId, x.TipoId, x.Uso); return View(x); }

            try
            {
                var id = _repo.Create(x);
                TempData["Ok"] = "Inmueble creado.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"Error MySQL: {ex.Message}");
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }
        }

        // GET: /Inmuebles/Edit/5
        public IActionResult Edit(int id)
        {
            var x = _repo.GetById(id);
            if (x == null) return NotFound();
            CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
            return View(x);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble x)
        {
            if (id != x.Id) return BadRequest();
            if (x.PropietarioId <= 0) ModelState.AddModelError(nameof(x.PropietarioId), "Seleccione un propietario.");
            if (x.TipoId <= 0) ModelState.AddModelError(nameof(x.TipoId), "Seleccione un tipo.");
            if (string.IsNullOrWhiteSpace(x.Direccion)) ModelState.AddModelError(nameof(x.Direccion), "Dirección obligatoria.");
            if (x.Ambientes < 1) ModelState.AddModelError(nameof(x.Ambientes), "Ambientes debe ser ≥ 1.");
            if (x.PrecioBase < 0) ModelState.AddModelError(nameof(x.PrecioBase), "Precio inválido.");
            if (!ModelState.IsValid) { CargarCombos(x.PropietarioId, x.TipoId, x.Uso); return View(x); }

            try
            {
                var rows = _repo.Update(x);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Inmueble actualizado.";
                return RedirectToAction(nameof(Details), new { id = x.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                CargarCombos(x.PropietarioId, x.TipoId, x.Uso);
                return View(x);
            }
        }

        // GET: /Inmuebles/Delete/5
        public IActionResult Delete(int id)
        {
            var x = _repo.GetById(id);
            return x == null ? NotFound() : View(x);
        }

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
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var x = _repo.GetById(id);
                return x == null ? NotFound() : View("Delete", x);
            }
        }
    }
}
