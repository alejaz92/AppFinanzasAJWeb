using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;
using AppFinanzasWeb.Servicios;
using AppFinanzasWeb.ViewModels;

namespace AppFinanzasWeb.Controllers
{
    public class ClaseMovimientoController : Controller
    {
        private readonly IRepositorioClaseMovimientos repositorioClaseMovimientos;

        public ClaseMovimientoController(IRepositorioClaseMovimientos repositorioClaseMovimientos)
        {
            this.repositorioClaseMovimientos = repositorioClaseMovimientos;
        }

        public async Task<IActionResult> Index()
        {
            var claseMovimientos = await repositorioClaseMovimientos.Obtener();
            var ingresos = claseMovimientos.Where(cm => cm.IngEgr == "Ingreso").ToList();
            var egresos = claseMovimientos.Where(cm => cm.IngEgr == "Egreso").ToList();

            var vmClaseMov = new ClaseMovimientoTipoViewModel
            {
                Ingresos = ingresos,
                Egresos = egresos,
            };

            return View(vmClaseMov);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ClaseMovimiento claseMovimiento)
        {
            if (!ModelState.IsValid)
            {
                return View(claseMovimiento);
            }

            await repositorioClaseMovimientos.Crear(claseMovimiento);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var claseMovimiento = await repositorioClaseMovimientos.ObtenerPorId(id);

            if (claseMovimiento is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(claseMovimiento);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ClaseMovimiento claseMovimiento)
        {
            var claseMovimientoExiste = await repositorioClaseMovimientos.ObtenerPorId(claseMovimiento.Id);

            if (claseMovimientoExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(claseMovimientoExiste);
            }

            await repositorioClaseMovimientos.Actualizar(claseMovimiento);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteClaseMovimiento(string nombre)
        {
            var yaExisteCuenta = await repositorioClaseMovimientos.Existe(nombre);

            if (yaExisteCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            var cuenta = await repositorioClaseMovimientos.ObtenerPorId(id);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioClaseMovimientos.Borrar(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> checkEsUsado(int id)
        {
            var cuenta = await repositorioClaseMovimientos.ObtenerPorId(id);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return Json(new { controlador = "ClaseMovimiento", result = await repositorioClaseMovimientos.EsUsado(id) });
        }
    }
}
