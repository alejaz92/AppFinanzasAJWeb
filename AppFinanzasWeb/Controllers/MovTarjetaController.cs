using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Servicios;
using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppFinanzasWeb.ViewModels;


namespace AppFinanzasWeb.Controllers
{
    public class MovTarjetaController : Controller
    {
        private readonly IRepositorioMovTarjetas repositorioMovTarjetas;
        private readonly IRepositorioActivos repositorioActivos;
        private readonly IRepositorioClaseMovimientos repositorioClaseMovimientos;
        private readonly IRepositorioTarjetas repositorioTarjetas;
        public MovTarjetaController(IRepositorioMovTarjetas repositorioMovTarjetas, IRepositorioActivos repositorioActivos, 
            IRepositorioClaseMovimientos repositorioClaseMovimientos, IRepositorioTarjetas repositorioTarjetas)
        {
            this.repositorioMovTarjetas = repositorioMovTarjetas;
            this.repositorioActivos = repositorioActivos;
            this.repositorioClaseMovimientos = repositorioClaseMovimientos;
            this.repositorioTarjetas = repositorioTarjetas;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            int cantidadPorPaginas = 20;
            int totalMovimientos = await repositorioMovTarjetas.ObtenerTotalMovimientos();
            int totalPaginas = (int)Math.Ceiling(totalMovimientos / (double)cantidadPorPaginas);

            var movimientos = await repositorioMovTarjetas.ObtenerMovimientosPaginacion(pagina, cantidadPorPaginas);

            var viewModel = new MovTarjetaIndexViewModel
            {
                Movimientos = movimientos,
                PaginaActual = pagina,
                PaginasTotales = totalPaginas
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Crear()
        {
            ViewBag.Activos = await repositorioActivos.ObtenerPorTipo("Moneda");
            var clasesMovimiento = await repositorioClaseMovimientos.Obtener();
            ViewBag.ClasesMovimiento = clasesMovimiento.Where(cm => cm.IngEgr =="Egreso").ToList();
            ViewBag.Tarjetas = await repositorioTarjetas.Obtener();

            return View();
        }

    }
}
