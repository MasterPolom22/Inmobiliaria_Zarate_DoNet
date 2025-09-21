using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Utils;
using Microsoft.AspNetCore.Mvc;

[AuthorizeLogin]
public class PagosController : Controller
{
    private readonly PagoRepository _repo;
    public PagosController(PagoRepository repo) => _repo = repo;

    public IActionResult Details(int id)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        return View(p);
    }

    public IActionResult Create(int contratoId)
    {
        return View(new Pago { ContratoId = contratoId, Fecha = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Pago p)
    {
        if (p.Importe <= 0) ModelState.AddModelError(nameof(p.Importe), "El importe debe ser mayor a cero");
        if (!ModelState.IsValid) return View(p);

        p.CreadoPor = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
        var id = _repo.Create(p);
        TempData["Ok"] = "Pago registrado";
        return RedirectToAction(nameof(Details), new { id });
    }

    [AuthorizeRol(Roles="ADMIN")]
    public IActionResult Anular(int id)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        if (p.Anulado) { TempData["Ok"] = "El pago ya estaba anulado."; return RedirectToAction(nameof(Details), new { id }); }
        return View(p);
    }

    [HttpPost, ActionName("Anular")]
    [ValidateAntiForgeryToken]
    [AuthorizeRol(Roles="ADMIN")]
    public IActionResult AnularConfirmed(int id)
    {
        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
        _repo.Anular(id, userId);
        TempData["Ok"] = "Pago anulado";
        return RedirectToAction(nameof(Details), new { id });
    }
}
