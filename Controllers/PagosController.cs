using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    public class PagosController : Controller
    {
        private readonly PagoRepository _repo;
        private readonly ContratoRepository _contratos;

        public PagosController(PagoRepository repo, ContratoRepository contratos)
        {
            _repo = repo;
            _contratos = contratos;
        }

        // INDEX por contrato
        // /Pagos?contratoId=123
        public IActionResult Index(int contratoId)
        {
            var pagos = _repo.GetByContrato(contratoId);
            ViewBag.ContratoId = contratoId;

            // cabecera opcional (dirección / inquilino)
            var c = _contratos.GetById(contratoId);
            ViewBag.ContratoResumen = c == null
                ? ""
                : $"{c.InmuebleDireccion} – {c.InquilinoNombre}";

            return View(pagos);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // CREATE GET
        public IActionResult Create(int contratoId)
        {
            var p = new Pago
            {
                ContratoId = contratoId,
                Fecha = DateTime.Today
            };
            return View(p);
        }

        // CREATE POST
        [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Pago p)
{
    if (p.ContratoId <= 0) ModelState.AddModelError(nameof(p.ContratoId), "Contrato inválido.");
    if (p.Fecha == default) ModelState.AddModelError(nameof(p.Fecha), "Fecha requerida.");
    if (string.IsNullOrWhiteSpace(p.Detalle) || p.Detalle.Length > 200)
        ModelState.AddModelError(nameof(p.Detalle), "Detalle obligatorio (máx. 200).");
    if (p.Importe < 0) ModelState.AddModelError(nameof(p.Importe), "Importe debe ser ≥ 0.");

    if (!ModelState.IsValid) return View(p);

    p.CreadoPor = 1; // TODO: tomar del usuario autenticado

    try
    {
        var id = _repo.Create(p);
        TempData["Ok"] = "Pago registrado.";
        return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
    }
    catch (MySqlException ex) when (ex.Number == 1644) // por si hay SIGNALs en tu BD
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


        // ANULAR GET (confirmación)
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

        // ANULAR POST
        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public IActionResult AnularConfirmed(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();

            try
            {
                int usuarioId = 1; // TODO: tomar del usuario autenticado cuando exista
                var rows = _repo.Anular(id, usuarioId);
                if (rows == 0) return NotFound();

                TempData["Ok"] = "Pago anulado correctamente.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            catch (MySqlException ex) when (ex.Number == 1644)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Anular", p);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View("Anular", p);
            }
        }
    }
}
