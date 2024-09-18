using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System;
using AppFinanzasWeb.ViewModels.Estadistica;

namespace AppFinanzasWeb.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly IRepositorioMovimientos repositorioMovimientos;
        private readonly IRepositorioTarjetas repositorioTarjetas;
        private readonly IRepositorioMovTarjetas repositorioMovTarjetas;

        public EstadisticaController(IRepositorioMovimientos repositorioMovimientos, IRepositorioTarjetas repositorioTarjetas, IRepositorioMovTarjetas repositorioMovTarjetas)
        {
            this.repositorioMovimientos = repositorioMovimientos;
            this.repositorioTarjetas = repositorioTarjetas;
            this.repositorioMovTarjetas = repositorioMovTarjetas;
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.MesActual = DateTime.Now.ToString("yyyy-MM"); // Formato 'YYYY-MM' requerido por el input de tipo 'month'.
            ViewBag.Tarjetas = await repositorioTarjetas.Obtener();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> getDataDB1([FromBody]string selectedMonth)
        {
            var year = int.Parse(selectedMonth.Split('-')[0]);
            var month = int.Parse(selectedMonth.Split('-')[1]);

            var ingresosPorClase = await repositorioMovimientos.ObtenerIngresosPorClase(year, month);
            var ingresosUltMeses = await repositorioMovimientos.ObtenerIngresosUltMeses();
            var egresosPorClase = await repositorioMovimientos.ObtenerEgresosPorClase(year, month);
            var egresosUltMeses = await repositorioMovimientos.ObtenerEgresosUltMeses();

            var result = new
            {
                ingresosPorClase = ingresosPorClase,
                ingresosUltMeses = ingresosUltMeses,
                egresosPorClase =  egresosPorClase,
                egresosUltMeses = egresosUltMeses
            };
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> getDataDB2([FromBody] string selectedMonth)
        {
            var year = int.Parse(selectedMonth.Split('-')[0]);
            var month = int.Parse(selectedMonth.Split('-')[1]);

            var ingresosPorClase = await repositorioMovimientos.ObtenerIngresosPorClasePesos(year, month);
            var ingresosUltMeses = await repositorioMovimientos.ObtenerIngresosUltMesesPesos();
            var egresosPorClase = await repositorioMovimientos.ObtenerEgresosPorClasePesos(year, month);
            var egresosUltMeses = await repositorioMovimientos.ObtenerEgresosUltMesesPesos();

            var result = new
            {
                ingresosPorClase = ingresosPorClase,
                ingresosUltMeses = ingresosUltMeses,
                egresosPorClase = egresosPorClase,
                egresosUltMeses = egresosUltMeses
            };
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> getDataDB3([FromBody] string selectedCard)
        {
            IEnumerable<MovimientoUlt6MesesViewModel> tarjetaPesos = null;
            IEnumerable<MovimientoUlt6MesesViewModel> tarjetaDolares = null;


            if (selectedCard == "Total")
            {
                tarjetaPesos = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMesesTotal( "Peso Argentino");
                tarjetaDolares = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMesesTotal("Dolar Estadounidense");
            }
            else
            {
                tarjetaPesos = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMeses(selectedCard, "Peso Argentino");
                tarjetaDolares = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMeses(selectedCard, "Dolar Estadounidense");
            }


            var result = new
            {
                tarjetaPesos = tarjetaPesos,
                tarjetaDolares = tarjetaDolares
            };
            return Json(result);
        }
    }
}
