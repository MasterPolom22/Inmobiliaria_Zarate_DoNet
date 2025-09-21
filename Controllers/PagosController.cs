using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class PagosController : Controller
    {
        private readonly PagoRepository _repo;
        private readonly ContratoRepository _contratos;
        public PagosController(PagoRepository repo, ContratoRepository contratos)
        {
            _repo = repo;
            _contratos = contratos;
        }

        // Lista por contrato
        public IActionResult Index(int contratoId)
        {
            var pagos = _repo.GetByContrato(contratoId);
            ViewBag.ContratoId = contratoId;
            var c = _contratos.GetById(contratoId);
            ViewBag.ContratoResumen = c == null ? "" : $"{c.InmuebleDireccion} – {c.InquilinoNombre}";
            return View(pagos);
        }

        public IActionResult Details(int id)
        {
            var p = _repo.GetById(id);
            return p == null ? NotFound() : View(p);
        }

        public IActionResult Create(int contratoId) =>
            View(new Pago { ContratoId = contratoId, Fecha = DateTime.Today });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago p)
        {
            if (p.ContratoId <= 0) ModelState.AddModelError(nameof(p.ContratoId), "Contrato inválido.");
            if (p.Fecha == default) ModelState.AddModelError(nameof(p.Fecha), "Fecha requerida.");
            if (string.IsNullOrWhiteSpace(p.Detalle) || p.Detalle.Length > 200) ModelState.AddModelError(nameof(p.Detalle), "Detalle obligatorio (máx. 200).");
            if (p.Importe < 0) ModelState.AddModelError(nameof(p.Importe), "Importe debe ser ≥ 0.");
            if (!ModelState.IsValid) return View(p);

            p.CreadoPor = 1; // reemplazar con usuario logueado

            try
            {
                _repo.Create(p);
                TempData["Ok"] = "Pago registrado.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            catch (MySqlException ex) when (ex.Number == 1644)
            {
                ModelState.AddModelError("", ex.Message);
                return View(p);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(p);
            }
        }

        public IActionResult Anular(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            if (p.Anulado)
            {
                TempData["Ok"] = "El pago ya está anulado.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            return View(p);
        }

        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmed(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();

            try
            {
                _repo.Anular(id, anuladoPor: 1); // reemplazar con usuario logueado
                TempData["Ok"] = "Pago anulado.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View("Anular", p);
            }
        }

        public IActionResult Delete(int id)
        {
            var p = _repo.GetById(id);
            return p == null ? NotFound() : View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            try
            {
                _repo.Delete(id);
                TempData["Ok"] = "Pago eliminado.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                return View("Delete", p);
            }
        }
    }
}
