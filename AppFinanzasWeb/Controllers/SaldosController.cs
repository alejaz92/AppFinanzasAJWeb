using AppFinanzasWeb.Servicios;
using AppFinanzasWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.Controllers
{
    public class SaldosController : Controller
    {

        private readonly IRepositorioMovimientos repositorioMovimientos;
        private readonly IRepositorioActivos repositorioActivos;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioTiposActivo repositorioTiposActivo;

        public SaldosController(IRepositorioMovimientos repositorioMovimientos, IRepositorioCuentas repositorioCuentas, 
            IRepositorioActivos repositorioActivos, IRepositorioTiposActivo repositorioTiposActivo)
        {
            this.repositorioActivos = repositorioActivos;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioMovimientos = repositorioMovimientos;
            this.repositorioTiposActivo = repositorioTiposActivo ;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TiposActivo = await repositorioTiposActivo.Obtener();




            ViewBag.ActivosAccion = await repositorioActivos.ObtenerPorTipo("Accion Argentina");
            ViewBag.ActivosBonos = await repositorioActivos.ObtenerPorTipo("Bonos");
            ViewBag.ActivosCEDEAR = await repositorioActivos.ObtenerPorTipo("CEDEAR");
            ViewBag.ActivosFCI = await repositorioActivos.ObtenerPorTipo("FCI");

            ViewBag.ActivosMoneda = await repositorioActivos.ObtenerPorTipo("Moneda");
            ViewBag.ActivosCripto = await repositorioActivos.ObtenerPorTipo("Criptomoneda");


            var saldoPesos =  await repositorioMovimientos.getTotalEnPesos();
            var  saldoDolares =  await repositorioMovimientos.getTotalEnDolares();

            var saldosVM = new SaldosViewModel
            {
                SaldoPesos =  saldoPesos.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES")),
                SaldoDolares = saldoDolares.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES"))
            };



            return View(saldosVM);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarTablasSaldos(int idActivo)
        {
            Activo activo = await repositorioActivos.ObtenerPorId(idActivo);

            
            if (activo is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            TipoActivo tipo = await repositorioTiposActivo.ObtenerPorId(activo.IDTIPOACTIVO); 

            var listado = await repositorioMovimientos.GetMontosPorCuenta(activo.Id);


            var ctaTransformadas = listado.Select(cuenta => new
            {
                Cuenta = cuenta.Cuenta,
                Monto = cuenta.Monto.ToString() // Esta es una declaración inicial, pero se reasignará en los bloques if/else
            });

            if (tipo.Nombre == "Moneda")
            {
                ctaTransformadas = listado.Select(cuenta => new
                {
                    Cuenta = cuenta.Cuenta,
                    Monto = cuenta.Monto.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES"))
                });
            } else
            {
                ctaTransformadas = listado.Select(cuenta => new
                {
                    Cuenta = cuenta.Cuenta,
                    Monto = cuenta.Monto.ToString("#,##0.############################", new System.Globalization.CultureInfo("es-ES"))
            });
            }  
           
             

            return Json(ctaTransformadas);
        }
    }
}
