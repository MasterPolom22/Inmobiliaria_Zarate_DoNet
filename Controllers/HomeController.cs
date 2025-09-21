using System.Diagnostics;
using Inmobiliaria_Zarate_DoNet.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_Zarate_DoNet.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
