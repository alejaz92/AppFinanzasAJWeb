using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppFinanzasWeb.ViewModels;
using System.ClientModel.Primitives;


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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            decimal precioCotiz;

            if (model.IdActivo == 2)
            {
                precioCotiz = 1; 
            }
            else
            {
                precioCotiz = 0;
            }

            var idMovimiento = await repositorioMovimientos.ObtenerIdMaximo() + 1;

            if (model.TipoMovimiento == "Intercambio")
            {
                await repositorioMovimientos.InsertarMovimiento(new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = (int)model.IdCuentaIngreso,
                    IdActivo = model.IdActivo,
                    TipoMovimiento = model.TipoMovimiento,
                    IdClaseMovimiento = null,
                    Comentario = model.Detalle,
                    Monto = model.Monto,
                    Fecha = model.Fecha,
                    PrecioCotiz = precioCotiz
                });

                await repositorioMovimientos.InsertarMovimiento(new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = (int)model.IdCuentaEgreso,
                    IdActivo = model.IdActivo,
                    TipoMovimiento = model.TipoMovimiento,
                    IdClaseMovimiento = null,
                    Comentario = model.Detalle,
                    Monto = -model.Monto,
                    Fecha = model.Fecha,
                    PrecioCotiz = precioCotiz
                });
            }
            else
            {
                decimal monto = model.TipoMovimiento == "Egreso" ? -model.Monto : model.Monto;

                await repositorioMovimientos.InsertarMovimiento(new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = model.TipoMovimiento == "Ingreso" ? (int)model.IdCuentaIngreso : (int)model.IdCuentaEgreso,
                    IdActivo = model.IdActivo,
                    IdClaseMovimiento = model.TipoMovimiento == "Ingreso" ? (int)model.IdClaseIngreso : (int)model.IdClaseEgreso,
                    TipoMovimiento = model.TipoMovimiento,
                    Comentario = model.Detalle,
                    Monto = monto,
                    Fecha = model.Fecha,
                    PrecioCotiz = precioCotiz
                });
            }

            TempData["SuccessMessage"] = "Movimiento registrado con éxito.";
            return RedirectToAction(nameof(Crear));
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var movimiento = await repositorioMovimientos.ObtenerPorId(id);

            if (movimiento is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioMovimientos.Borrar(movimiento.IdMovimiento);
            return Ok();
        }

        [HttpGet]
        public IActionResult checkEsUsado(int id)
        {

            return Json(new { controlador = "Movimiento", result = false });
        }

        [HttpGet]

        public async Task<IActionResult> Reintegro(int id)
        {
            var movimiento = await repositorioMovimientos.ObtenerPorId(id);

            var cuentas = await repositorioCuentas.ObtenerPorTipo("Moneda");

            if (movimiento is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuentas = cuentas;

            var viewModel = new MovimientoReintegroViewModel
            {
                movimiento = movimiento
            };

            return View(viewModel);
        }
    }
}
