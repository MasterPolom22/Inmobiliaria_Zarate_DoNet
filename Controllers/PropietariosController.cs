using Inmobiliaria_Zarate_DoNet.Data;
using Inmobiliaria_Zarate_DoNet.Filters;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Text.RegularExpressions;
namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    [AuthorizeLogin]
    public class PropietariosController : Controller
    {
        private readonly PropietarioRepository _repo;
        private readonly InmuebleRepository _repoInmueble; 

        public PropietariosController(PropietarioRepository repo, InmuebleRepository repoInmueble)
        {
            _repo = repo;
            _repoInmueble = repoInmueble;
        }

        public IActionResult Index() => View(_repo.GetAll());

        public IActionResult Details(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound();

            // Traer inmuebles del propietario para el panel secundario
            var inmuebles = _repoInmueble.GetByPropietario(id);
            ViewBag.Inmuebles = inmuebles;

            return View(p);
        }

       public IActionResult Create() => View();

// POST
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Propietario p)
{
    // Normalización
    p.Dni = (p.Dni ?? "").Trim();
    p.Apellido = (p.Apellido ?? "").Trim();
    p.Nombre = (p.Nombre ?? "").Trim();
    p.Email = (p.Email ?? "").Trim().ToLowerInvariant();
    p.Telefono = (p.Telefono ?? "").Trim();

    // Reglas
    if (string.IsNullOrWhiteSpace(p.Dni))
        ModelState.AddModelError(nameof(p.Dni), "DNI es obligatorio");
    else if (!Regex.IsMatch(p.Dni, @"^\d{7,10}$"))
        ModelState.AddModelError(nameof(p.Dni), "DNI debe tener solo números (7 a 10 dígitos)");

    if (string.IsNullOrWhiteSpace(p.Apellido))
        ModelState.AddModelError(nameof(p.Apellido), "Apellido es obligatorio");

    if (string.IsNullOrWhiteSpace(p.Nombre))
        ModelState.AddModelError(nameof(p.Nombre), "Nombre es obligatorio");

    if (string.IsNullOrWhiteSpace(p.Email))
        ModelState.AddModelError(nameof(p.Email), "Email es obligatorio");
    else
    {
        try { var addr = new MailAddress(p.Email); if (addr.Address != p.Email) throw new FormatException(); }
        catch { ModelState.AddModelError(nameof(p.Email), "Email no es válido"); }
    }

    if (string.IsNullOrWhiteSpace(p.Telefono))
        ModelState.AddModelError(nameof(p.Telefono), "Teléfono es obligatorio");
    else if (!Regex.IsMatch(p.Telefono, @"^\+?\d{7,15}$"))
        ModelState.AddModelError(nameof(p.Telefono), "Teléfono: solo números (7 a 15 dígitos), opcional +");

    // Duplicados
    if (_repo.ExistsDni(p.Dni))
        ModelState.AddModelError(nameof(p.Dni), "DNI ya registrado");
    

    if (!ModelState.IsValid) return View(p);

    try
    {
        var id = _repo.Create(p);
        TempData["Ok"] = "Propietario creado.";
        return RedirectToAction(nameof(Details), new { id });
    }
    catch (MySqlException ex) when (ex.Number == 1062)
    {
        // Si el índice único es por DNI, marco ese campo; si es por email/teléfono
        ModelState.AddModelError(nameof(p.Dni), "Registro duplicado (índice único).");
        return View(p);
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", $"Error: {ex.Message}");
        return View(p);
    }
}

// GET
public IActionResult Edit(int id)
{
    var p = _repo.GetById(id);
    return p == null ? NotFound() : View(p);
}

// POST
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Propietario p)
{
    if (id != p.Id) return BadRequest();

    // Normalización
    p.Dni = (p.Dni ?? "").Trim();
    p.Apellido = (p.Apellido ?? "").Trim();
    p.Nombre = (p.Nombre ?? "").Trim();
    p.Email = (p.Email ?? "").Trim().ToLowerInvariant();
    p.Telefono = (p.Telefono ?? "").Trim();

    // Reglas
    if (string.IsNullOrWhiteSpace(p.Dni))
        ModelState.AddModelError(nameof(p.Dni), "DNI es obligatorio");
    else if (!Regex.IsMatch(p.Dni, @"^\d{7,10}$"))
        ModelState.AddModelError(nameof(p.Dni), "DNI debe tener solo números (7 a 10 dígitos)");

    if (string.IsNullOrWhiteSpace(p.Apellido))
        ModelState.AddModelError(nameof(p.Apellido), "Apellido es obligatorio");

    if (string.IsNullOrWhiteSpace(p.Nombre))
        ModelState.AddModelError(nameof(p.Nombre), "Nombre es obligatorio");

    if (string.IsNullOrWhiteSpace(p.Email))
        ModelState.AddModelError(nameof(p.Email), "Email es obligatorio");
    else
    {
        try { var addr = new MailAddress(p.Email); if (addr.Address != p.Email) throw new FormatException(); }
        catch { ModelState.AddModelError(nameof(p.Email), "Email no es válido"); }
    }

    if (string.IsNullOrWhiteSpace(p.Telefono))
        ModelState.AddModelError(nameof(p.Telefono), "Teléfono es obligatorio");
    else if (!Regex.IsMatch(p.Telefono, @"^\+?\d{7,15}$"))
        ModelState.AddModelError(nameof(p.Telefono), "Teléfono: solo números (7 a 15 dígitos), opcional +");

    // Duplicados (excluyendo a mí mismo)
    if (_repo.ExistsDni(p.Dni, excludeId: id))
        ModelState.AddModelError(nameof(p.Dni), "DNI ya registrado en otro");
    

    if (!ModelState.IsValid) return View(p);

    try
    {
        var rows = _repo.Update(p);
        if (rows == 0) return NotFound();
        TempData["Ok"] = "Propietario actualizado.";
        return RedirectToAction(nameof(Details), new { id = p.Id });
    }
    catch (MySqlException ex) when (ex.Number == 1062)
    {
        ModelState.AddModelError(nameof(p.Dni), "Registro duplicado (índice único).");
        return View(p);
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", $"Error: {ex.Message}");
        return View(p);
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
            try
            {
                var rows = _repo.Delete(id);
                if (rows == 0) return NotFound();
                TempData["Ok"] = "Propietario eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"No se puede eliminar: {ex.Message}");
                var p = _repo.GetById(id);
                return p == null ? NotFound() : View("Delete", p);
            }
        }
    }
}
