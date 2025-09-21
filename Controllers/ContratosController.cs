using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class ContratosController : Controller
    {
        private readonly ContratoRepository _repo;
        public ContratosController(ContratoRepository repo) => _repo = repo;

        // Combos
        private void CargarCombos(int? inmuebleId = null, int? inquilinoId = null)
        {
            var inm = _repo.GetInmueblesForSelect().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto }).ToList();
            var inq = _repo.GetInquilinosForSelect().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto }).ToList();
            ViewBag.Inmuebles = new SelectList(inm, "Value", "Text", inmuebleId?.ToString());
            ViewBag.Inquilinos = new SelectList(inq, "Value", "Text", inquilinoId?.ToString());
        }

        public IActionResult Index() => View(_repo.GetAll());

        public IActionResult Details(int id)
        {
            var c = _repo.GetById(id);
            return c == null ? NotFound() : View(c);
        }

        public IActionResult Create()
        {
            CargarCombos();
            return View(new Contrato
            {
                FechaInicio = DateTime.Today,
                FechaFinOriginal = DateTime.Today.AddMonths(12),
                MontoMensual = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato c)
        {
            if (c.InmuebleId <= 0) ModelState.AddModelError(nameof(c.InmuebleId), "Seleccione un inmueble.");
            if (c.InquilinoId <= 0) ModelState.AddModelError(nameof(c.InquilinoId), "Seleccione un inquilino.");
            if (c.MontoMensual <= 0) ModelState.AddModelError(nameof(c.MontoMensual), "Monto mensual debe ser > 0.");
            if (c.FechaFinOriginal <= c.FechaInicio) ModelState.AddModelError(nameof(c.FechaFinOriginal), "Fin debe ser mayor que inicio.");
            if (c.FechaFinAnticipada.HasValue && c.FechaFinAnticipada <= c.FechaInicio)
                ModelState.AddModelError(nameof(c.FechaFinAnticipada), "Fin anticipado debe ser mayor que inicio.");
            if (!ModelState.IsValid) { CargarCombos(c.InmuebleId, c.InquilinoId); return View(c); }

            try
            {
                if (c.CreadoPor == 0) c.CreadoPor = 1; // reemplazar con usuario logueado
                var id = _repo.Create(c);
                TempData["Ok"] = "Contrato creado.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (MySqlException ex) when (ex.Number == 1644)
            {
                ModelState.AddModelError("", ex.Message); // p.ej. solapamiento
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }
        }

        public IActionResult Edit(int id)
        {
            var c = _repo.GetById(id);
            if (c == null) return NotFound();
            CargarCombos(c.InmuebleId, c.InquilinoId);
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato c)
        {
            if (id != c.Id) return BadRequest();
            if (c.InmuebleId <= 0) ModelState.AddModelError(nameof(c.InmuebleId), "Seleccione un inmueble.");
            if (c.InquilinoId <= 0) ModelState.AddModelError(nameof(c.InquilinoId), "Seleccione un inquilino.");
            if (c.MontoMensual <= 0) ModelState.AddModelError(nameof(c.MontoMensual), "Monto mensual debe ser > 0.");
            if (c.FechaFinOriginal <= c.FechaInicio) ModelState.AddModelError(nameof(c.FechaFinOriginal), "Fin debe ser mayor que inicio.");
            if (c.FechaFinAnticipada.HasValue && c.FechaFinAnticipada <= c.FechaInicio)
                ModelState.AddModelError(nameof(c.FechaFinAnticipada), "Fin anticipado debe ser mayor que inicio.");
            if (!ModelState.IsValid) { CargarCombos(c.InmuebleId, c.InquilinoId); return View(c); }

            try
            {
                var rows = _repo.Update(c);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Contrato actualizado.";
                return RedirectToAction(nameof(Details), new { id = c.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }
        }

        public IActionResult Delete(int id)
        {
            var c = _repo.GetById(id);
            return c == null ? NotFound() : View(c);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Contrato eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var c = _repo.GetById(id);
                return c == null ? NotFound() : View("Delete", c);
            }
        }
    }
}
