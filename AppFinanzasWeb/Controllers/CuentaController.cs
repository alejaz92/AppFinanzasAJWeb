using AppFinanzasWeb.Models;
using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Linq;

namespace AppFinanzasWeb.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioTiposActivo repositorioTiposActivo;

        public CuentaController(IRepositorioCuentas repositorioCuentas, IRepositorioTiposActivo repositorioTiposActivo)
        {
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioTiposActivo = repositorioTiposActivo;
        }
        public async Task<IActionResult> Index()
        {
            var cuentas = await repositorioCuentas.Obtener();
            return View(cuentas);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Cuenta cuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(cuenta);
            }
            //continuo con el crear mientras
            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteCuenta(string nombre)
        {
            var yaExisteCuenta = await repositorioCuentas.Existe(nombre);

            if (yaExisteCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int Id)
        {
            var cuenta = await repositorioCuentas.ObtenerPorId(Id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Cuenta cuenta)
        {
            var cuentaExiste = await repositorioCuentas.ObtenerPorId(cuenta.Id);

            if (cuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(cuenta);
            }

            await repositorioCuentas.Actualizar(cuenta);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public  async Task<IActionResult> Borrar(int id)
        {
            var cuenta = await repositorioCuentas.ObtenerPorId(id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Borrar(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> checkEsUsado(int id)
        {
            var cuenta = await repositorioCuentas.ObtenerPorId(id);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return Json(new { controlador = "Cuenta", result = await repositorioCuentas.EsUsado(id) });
        }

        public async Task<IActionResult> AsignarTiposActivos(int Id)
        {
            var cuenta = await repositorioCuentas.ObtenerPorId(Id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tiposActivosAsignados = await repositorioTiposActivo.ObtenerPorCuenta(Id);
            var tiposActivosAsignadosIds = tiposActivosAsignados.Select(ta => ta.IdTipoActivo).ToList();

            var tiposActivos = await repositorioTiposActivo.Obtener();

            ViewBag.Cuenta = cuenta;
            ViewBag.TiposActivosAsignados = tiposActivosAsignadosIds;
            return View(tiposActivos);
        }

        [HttpPost]
        public async Task<IActionResult> AsignarTiposActivos(int id, List<int> IdTipoActivos)
        {
            var success = await repositorioTiposActivo.ActualizarCuentaTiposActivos(id, IdTipoActivos);

            if(!success)
            {
                TempData["Error"] = "No se puede desasignar un tipo de activo que tiene registros existentes. ";
                return RedirectToAction("AsignarTiposActivos", new { id });
            }

            return RedirectToAction("Index");
        }
    }

}

