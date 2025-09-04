using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    public class ContratosController : Controller
    {
        private readonly ContratoRepository _repo;

        public ContratosController(ContratoRepository repo) => _repo = repo;

        // Helpers para combos
        private void CargarCombos(int? inmuebleSel = null, int? inquilinoSel = null)
        {
            var inm = _repo.GetInmueblesForSelect()
                           .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto }).ToList();
            var inq = _repo.GetInquilinosForSelect()
                           .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Texto }).ToList();

            ViewBag.Inmuebles = new SelectList(inm, "Value", "Text", inmuebleSel?.ToString());
            ViewBag.Inquilinos = new SelectList(inq, "Value", "Text", inquilinoSel?.ToString());
        }

        // INDEX
        public IActionResult Index()
        {
            var lista = _repo.GetAll();
            return View(lista);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var c = _repo.GetById(id);
            if (c == null) return NotFound();
            return View(c);
        }

        // CREATE GET
        public IActionResult Create()
        {
            CargarCombos();
            return View(new Contrato
            {
                FechaInicio = DateTime.Today,
                FechaFinOriginal = DateTime.Today.AddMonths(12),
                MontoMensual = 0,
                CreadoPor = 1 // TODO: tomar del usuario logueado cuando haya auth
            });
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato c)
        {
            // Validaciones básicas UI (doble red con triggers)
            if (c.InmuebleId <= 0) ModelState.AddModelError(nameof(c.InmuebleId), "Seleccione un inmueble.");
            if (c.InquilinoId <= 0) ModelState.AddModelError(nameof(c.InquilinoId), "Seleccione un inquilino.");
            if (c.MontoMensual <= 0) ModelState.AddModelError(nameof(c.MontoMensual), "Monto mensual debe ser > 0.");
            if (c.FechaFinOriginal <= c.FechaInicio) ModelState.AddModelError(nameof(c.FechaFinOriginal), "Fin debe ser mayor que inicio.");
            if (c.FechaFinAnticipada.HasValue && c.FechaFinAnticipada <= c.FechaInicio)
                ModelState.AddModelError(nameof(c.FechaFinAnticipada), "Fin anticipado debe ser mayor que inicio.");

            if (!ModelState.IsValid)
            {
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }

            try
            {
                if (c.CreadoPor == 0) c.CreadoPor = 1; // placeholder
                var id = _repo.Create(c);
                TempData["Ok"] = "Contrato creado correctamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (MySqlException ex) when (ex.Number == 1644) // SIGNAL 45000 (SP/triggers)
            {
                ModelState.AddModelError("", ex.Message); // Ej: Solapamiento
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }
            catch (MySqlException ex) when (ex.Number == 1062) // duplicado
            {
                ModelState.AddModelError("", "Registro duplicado.");
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

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var c = _repo.GetById(id);
            if (c == null) return NotFound();
            CargarCombos(c.InmuebleId, c.InquilinoId);
            return View(c);
        }

        // EDIT POST
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

            if (!ModelState.IsValid)
            {
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }

            try
            {
                var rows = _repo.Update(c); // triggers validan
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Contrato actualizado.";
                return RedirectToAction(nameof(Details), new { id = c.Id });
            }
            catch (MySqlException ex) when (ex.Number == 1644)
            {
                ModelState.AddModelError("", ex.Message);
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

        // DELETE GET
        public IActionResult Delete(int id)
        {
            var c = _repo.GetById(id);
            if (c == null) return NotFound();
            return View(c);
        }

        // DELETE POST
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
            catch (MySqlException ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var c = _repo.GetById(id);
                if (c == null) return NotFound();
                return View("Delete", c);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                var c = _repo.GetById(id);
                if (c == null) return NotFound();
                return View("Delete", c);
            }
        }
    }
}
