using AppFinanzasWeb.Models;
using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace AppFinanzasWeb.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepositorioCuentas repositorioCuentas;

        public CuentaController(IRepositorioCuentas repositorioCuentas)
        {
            this.repositorioCuentas = repositorioCuentas;
        }
        public async Task<IActionResult> Index()
        {
            var cuentas = await repositorioCuentas.Obtener();
            return View(cuentas);
        }
    }
}
