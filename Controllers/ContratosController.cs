using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Inmobiliaria_Zarate_DoNet.Utils;
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
            if (c == null) return NotFound();

            // NEW: nombres “Apellido, Nombre” para auditoría
            ViewBag.CreadoPorNombre = _repo.GetUsuarioNombre(c.CreadoPor);
            ViewBag.TerminadoPorNombre = _repo.GetUsuarioNombre(c.TerminadoPor);

            return View(c);
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

            // Validación de solapamiento (antes de intentar crear)
            if (_repo.ExisteSolapamiento(c.InmuebleId, c.FechaInicio, c.FechaFinOriginal))
            {
                ModelState.AddModelError("", "Existe un contrato que se superpone con el rango de fechas.");
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }

            try
            {
                var userId = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
                if (userId == 0) userId = 1; // fallback mínimo si no hay session (no debería pasar)
                c.CreadoPor = userId;
                var id = _repo.Create(c);
                TempData["Ok"] = "Contrato creado.";
                return RedirectToAction(nameof(Details), new { id });
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

            // Validación de solapamiento excluyendo este contrato
            if (_repo.ExisteSolapamiento(c.InmuebleId, c.FechaInicio, c.FechaFinOriginal, excludeId: c.Id))
            {
                ModelState.AddModelError("", "Existe un contrato que se superpone con el rango de fechas.");
                CargarCombos(c.InmuebleId, c.InquilinoId);
                return View(c);
            }

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
        [AuthorizeRol(Roles = "ADMIN")]
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

        // ------------------- Terminar contrato -------------------
        // GET: /Contratos/Terminar/5
        public IActionResult Terminar(int id)
        {
            var c = _repo.GetById(id);
            if (c == null) return NotFound();
            return View(c); // la vista pedirá la fecha efectiva de finalización
        }

        // POST: /Contratos/Terminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRol(Roles = "ADMIN")] // solo admin puede terminar/forzar finalización
        public IActionResult TerminarConfirmado(int id, DateTime fechaFinAnticipada)
        {
            var c = _repo.GetById(id);
            if (c == null) return NotFound();

            if (fechaFinAnticipada < c.FechaInicio)
            {
                ModelState.AddModelError("", "La fecha de finalización no puede ser anterior al inicio.");
                return View("Terminar", c);
            }

            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
            _repo.Terminar(id, fechaFinAnticipada, userId);
            TempData["Ok"] = "Contrato finalizado";
            return RedirectToAction(nameof(Details), new { id });
        }
        // -----------------------------------------------------------
    }
}
