using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System;
using AppFinanzasWeb.ViewModels.Estadistica;
using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly IRepositorioMovimientos repositorioMovimientos;
        private readonly IRepositorioTarjetas repositorioTarjetas;
        private readonly IRepositorioMovTarjetas repositorioMovTarjetas;
        private readonly IRepositorioTiposActivo repositorioTiposActivo;

        public EstadisticaController(IRepositorioMovimientos repositorioMovimientos, IRepositorioTarjetas repositorioTarjetas, 
            IRepositorioMovTarjetas repositorioMovTarjetas, IRepositorioTiposActivo repositorioTiposActivo)
        {
            this.repositorioMovimientos = repositorioMovimientos;
            this.repositorioTarjetas = repositorioTarjetas;
            this.repositorioMovTarjetas = repositorioMovTarjetas;
            this.repositorioTiposActivo = repositorioTiposActivo;
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.MesActual = DateTime.Now.ToString("yyyy-MM"); // Formato 'YYYY-MM' requerido por el input de tipo 'month'.
            ViewBag.Tarjetas = await repositorioTarjetas.Obtener();
            ViewBag.TiposActivo = await repositorioTiposActivo.ObtenerBolsa();
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
            if (selectedCard != "Total")
            {
                Tarjeta tarjeta = await repositorioTarjetas.ObtenerPorId(int.Parse(selectedCard));

                if (tarjeta is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

            }

            IEnumerable<MovimientoUlt6MesesViewModel> tarjetaPesos = null;
            IEnumerable<MovimientoUlt6MesesViewModel> tarjetaDolares = null;
            IEnumerable<MovTarjeta> gastosTarjetaMes = null;

            string mesActual = DateTime.Now.ToString("yyyy-MM") + "-01";

            if (selectedCard == "Total")
            {
                tarjetaPesos = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMesesTotal( "Peso Argentino");
                tarjetaDolares = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMesesTotal("Dolar Estadounidense");
                gastosTarjetaMes = await repositorioMovTarjetas.ObtenerMovimientosPagoTodas (mesActual);
            }
            else
            {
                tarjetaPesos = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMeses(selectedCard, "Peso Argentino");
                tarjetaDolares = await repositorioMovTarjetas.ObtenerEstadisticaTarjetaMeses(selectedCard, "Dolar Estadounidense");
                gastosTarjetaMes = await repositorioMovTarjetas.ObtenerMovimientosPago(int.Parse(selectedCard), mesActual);
            }

            var gastosTarjetaTransformados = gastosTarjetaMes.Select(movimiento => new
            {
                FechaMov = movimiento.FechaMov.ToString("yyyy-MM-dd"),
                NombreTarj = movimiento.NombreTarj,
                tipoMov = movimiento.TipoMov,
                detalle = movimiento.Detalle,
                nombreMoneda = movimiento.NombreMoneda,
                cuotaTexto = movimiento.CuotaTexto,
                montoCuota = movimiento.MontoCuota.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES")),
                valorPesos = movimiento.ValorPesos.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES")),
                idActivo = movimiento.IdActivo,
                idClaseMovimiento = movimiento.IdClaseMovimiento
            });

            var result = new
            {
                tarjetaPesos = tarjetaPesos,
                tarjetaDolares = tarjetaDolares,
                gastosTarjetaMes = gastosTarjetaTransformados
            };
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> getDataDB4([FromBody] int selectedTipoActivo)
        {
            TipoActivo tipoActivo = await repositorioTiposActivo.ObtenerPorId(selectedTipoActivo);

            if (tipoActivo is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var bolsaEstadistica1 = await repositorioMovimientos.ObtenerBolsaEstadistica(selectedTipoActivo);
            var bolsaEstadistica2 = await repositorioMovimientos.ObtenerBolsaEstadisticaGral();


            var result = new
            {
                bolsaEstadistica1 = bolsaEstadistica1,
                bolsaEstadistica2 = bolsaEstadistica2
            };
            return Json(result);
        }
    }
}
