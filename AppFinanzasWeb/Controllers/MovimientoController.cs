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
        private readonly IRepositorioActivos repositorioActivos;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioClaseMovimientos repositorioClaseMovimientos;

        public MovimientoController(IRepositorioMovimientos repositorioMovimientos, IRepositorioActivos repositorioActivos,
            IRepositorioCuentas repositorioCuentas, IRepositorioClaseMovimientos repositorioClaseMovimientos)
        {
            this.repositorioMovimientos = repositorioMovimientos;
            this.repositorioActivos = repositorioActivos;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioClaseMovimientos = repositorioClaseMovimientos;
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

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            ViewBag.Activos = await repositorioActivos.ObtenerPorTipo("Moneda");

            ViewBag.Cuentas = await repositorioCuentas.ObtenerPorTipo("Moneda");

            var claseMovimientos = await repositorioClaseMovimientos.Obtener();
            ViewBag.ClasesIngreso = claseMovimientos.Where(cm => cm.IngEgr == "Ingreso").ToList();
            ViewBag.ClasesEgreso = claseMovimientos.Where(cm => cm.IngEgr == "Egreso").ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(MovimientoViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var idMovimiento = await repositorioMovimientos.ObtenerIdMaximo() + 1;

            if ( model.TipoMovimiento == "Intercambio")
            {
                
            }
            else
            {

            }
        }

    }
}
