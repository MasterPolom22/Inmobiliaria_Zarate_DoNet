using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Models;
using Inmobiliaria_Zarate_DoNet.Filters;
using Microsoft.AspNetCore.Mvc;

[AuthorizeLogin]
public class TiposInmueblesController : Controller
{
    private readonly TipoInmuebleRepository _repo;
    public TiposInmueblesController(TipoInmuebleRepository repo) => _repo = repo;

    // GET: /TiposInmuebles
    public IActionResult Index() => View(_repo.GetAll());

    // GET: /TiposInmuebles/Details/5
    public IActionResult Details(int id)
    {
        var t = _repo.GetById(id);
        if (t == null) return NotFound();
        return View(t);
    }

    // GET: /TiposInmuebles/Create
    public IActionResult Create() => View();

    // POST: /TiposInmuebles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TipoInmueble t)
    {
        if (string.IsNullOrWhiteSpace(t.Nombre))
            ModelState.AddModelError(nameof(t.Nombre), "El nombre es obligatorio");

        if (!ModelState.IsValid) return View(t);

        var id = _repo.Create(t);
        TempData["Ok"] = "Tipo de inmueble creado";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /TiposInmuebles/Edit/5
    public IActionResult Edit(int id)
    {
        var t = _repo.GetById(id);
        if (t == null) return NotFound();
        return View(t);
    }

    // POST: /TiposInmuebles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TipoInmueble t)
    {
        if (id != t.Id) return BadRequest();

        if (string.IsNullOrWhiteSpace(t.Nombre))
            ModelState.AddModelError(nameof(t.Nombre), "El nombre es obligatorio");

        if (!ModelState.IsValid) return View(t);

        _repo.Update(t);
        TempData["Ok"] = "Tipo de inmueble actualizado";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /TiposInmuebles/Delete/5
    [AuthorizeRol(Roles="ADMIN")]
    public IActionResult Delete(int id)
    {
        var t = _repo.GetById(id);
        if (t == null) return NotFound();
        return View(t);
    }

    // POST: /TiposInmuebles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [AuthorizeRol(Roles="ADMIN")]
    public IActionResult DeleteConfirmed(int id)
    {
        _repo.Delete(id);
        TempData["Ok"] = "Tipo de inmueble eliminado";
        return RedirectToAction(nameof(Index));
    }
}
