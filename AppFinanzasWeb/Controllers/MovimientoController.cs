using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppFinanzasWeb.ViewModels;


namespace AppFinanzasWeb.Controllers
{
    public class MovimientoController : Controller
    {
        private readonly IRepositorioMovimientos repositorioMovimientos;

        public MovimientoController(IRepositorioMovimientos repositorioMovimientos)
        {
            this.repositorioMovimientos = repositorioMovimientos;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            int cantidadPorPaginas = 20;
            int totalMovimientos = await repositorioMovimientos.ObtenerTotalMovimientos();
            int totalPaginas = (int)Math.Ceiling(totalMovimientos / (double)cantidadPorPaginas);

            var movimientos = await repositorioMovimientos.ObtenerMovimientosPaginacion(pagina, cantidadPorPaginas);


            var viewModel = new MovimientosIndexViewModel
            {
                Movimientos = movimientos,
                PaginaActual = pagina,
                PaginasTotales = totalPaginas
            };
            return View(viewModel);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearMovRegular()
        {

        }

    }
}
