using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System;

namespace AppFinanzasWeb.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly IRepositorioMovimientos repositorioMovimientos;

        public EstadisticaController(IRepositorioMovimientos repositorioMovimientos)
        {
            this.repositorioMovimientos = repositorioMovimientos;
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.MesActual = DateTime.Now.ToString("yyyy-MM"); // Formato 'YYYY-MM' requerido por el input de tipo 'month'.
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> getDataDB1([FromBody]string selectedMonth)
        {
            var year = int.Parse(selectedMonth.Split('-')[0]);
            var month = int.Parse(selectedMonth.Split('-')[1]);

            var ingresosPorClase = await repositorioMovimientos.ObtenerIngresosPorClase(year, month);
            var egresosPorClase = await repositorioMovimientos.ObtenerEgresosPorClase(year, month);

            var result = new
            {
                ingresosPorClase = ingresosPorClase,
                egresosPorClase =  egresosPorClase
            };
            return Json(result);
        }
    }
}
