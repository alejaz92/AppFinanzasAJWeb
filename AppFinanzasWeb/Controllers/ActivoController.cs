using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.Controllers
{
    public class ActivoController: Controller
    {
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(Activo activo)
        {
            return View();
        }
    }
}
