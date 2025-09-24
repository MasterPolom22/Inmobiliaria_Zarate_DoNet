using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Utils;
using Microsoft.AspNetCore.Mvc;

[AuthorizeLogin]
public class PagosController : Controller
{
    private readonly PagoRepository _repo;
    private readonly ContratoRepository _contratoRepo; 
    public PagosController(PagoRepository repo, ContratoRepository contratoRepo) // ← DI con ambos
    {
        _repo = repo;
        _contratoRepo = contratoRepo;
    }
    

    // Listado por contrato y "alta" desde pantalla
    public IActionResult PorContrato(int contratoId)
    {
        var lista = _repo.GetByContrato(contratoId);
        ViewBag.ContratoId = contratoId;
        return View(lista);
    }

    // GET: /Pagos
    public IActionResult Index()
    {
        var contratos = _contratoRepo.GetAll(); // Id, InmuebleId, InquilinoId, FechaInicio, FechaFinOriginal
    return View(contratos);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(int contratoId)
    {
        var contratos = _contratoRepo.GetAll(); // o un método resumido
        return View(contratos);
    }

    // GET: /Pagos/Create?contratoId=5
    public IActionResult Create(int contratoId)
    {
        var p = new Pago { ContratoId = contratoId, Fecha = DateTime.Today };
        return View(p);
    }

    // POST: /Pagos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("ContratoId,Fecha,Detalle,Importe")] Pago p)
    {
        if (p.Importe <= 0) ModelState.AddModelError(nameof(p.Importe), "El importe debe ser mayor a cero.");
        if (p.Fecha == default) ModelState.AddModelError(nameof(p.Fecha), "La fecha es obligatoria.");

        if (!ModelState.IsValid) return View(p);

        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
        var id = _repo.Create(p, userId);
        TempData["Ok"] = "Pago registrado";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Pagos/Details/5
    public IActionResult Details(int id)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        return View(p);
    }

    // GET: /Pagos/Edit/5  (solo editar detalle)
    public IActionResult Edit(int id)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        if (p.Anulado)
        {
            TempData["Ok"] = "No se puede editar un pago anulado.";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(p);
    }

    // POST: /Pagos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, string Detalle)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        if (p.Anulado)
        {
            TempData["Ok"] = "No se puede editar un pago anulado.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (string.IsNullOrWhiteSpace(Detalle))
        {
            ModelState.AddModelError(nameof(Detalle), "El detalle es obligatorio.");
            return View(p);
        }

        _repo.UpdateDetalle(id, Detalle.Trim());
        TempData["Ok"] = "Detalle actualizado";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Pagos/Delete/5  (Confirmar ANULAR)
    [AuthorizeRol(Roles = "ADMIN")]
    public IActionResult Delete(int id)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        return View(p);
    }

    // POST: /Pagos/Delete/5  (ANULAR)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [AuthorizeRol(Roles = "ADMIN")]
    public IActionResult DeleteConfirmed(int id)
    {
        var p = _repo.GetById(id);
        if (p == null) return NotFound();
        if (p.Anulado)
        {
            TempData["Ok"] = "El pago ya estaba anulado.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId) ?? 0;
        _repo.Anular(id, userId);
        TempData["Ok"] = "Pago anulado";
        return RedirectToAction(nameof(Details), new { id });
    }
}
