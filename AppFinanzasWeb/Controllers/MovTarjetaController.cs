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


        [HttpPost] 
        public async Task<IActionResult> Crear(MovTarjeta model)
        {
            model.MontoTotal = Convert.ToDecimal(model.MontoTotalString);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.TipoMov == "recurrente")
            {
                model.Repite = "SI";
                model.IdMesUltimaCuota = 0;
                model.Cuotas = 1;
                model.MontoCuota = model.MontoTotal;
            } else
            {
                model.Repite = "NO";
                model.IdMesUltimaCuota = int.Parse(model.MesUltimaCuota.ToString("yyyyMMdd"));
                model.MontoCuota = model.MontoTotal / model.Cuotas;
            }

            model.IdFecha = int.Parse(model.FechaMov.ToString("yyyyMMdd"));
            model.IdMesPrimerCuota = int.Parse(model.MesPrimerCuota.ToString("yyyyMMdd"));
            


            await repositorioMovTarjetas.InsertarMovimiento(model);

            TempData["SuccessMessage"] = "Movimiento registrado con éxito.";
            return RedirectToAction(nameof(Crear));
        }
    }
}
