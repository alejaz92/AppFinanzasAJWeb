using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppFinanzasWeb.ViewModels;
using System.ClientModel.Primitives;
using System.Globalization;
using System.Reflection;


namespace AppFinanzasWeb.Controllers
{
    public class MovimientoController : Controller
    {
        private readonly IRepositorioMovimientos repositorioMovimientos;
        private readonly IRepositorioActivos repositorioActivos;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioClaseMovimientos repositorioClaseMovimientos;
        private readonly IRepositorioCotizacionesActivos repositorioCotizacionesActivos;
        private readonly IRepositorioTiposActivo repositorioTiposActivo;

        public MovimientoController(IRepositorioMovimientos repositorioMovimientos, IRepositorioActivos repositorioActivos,
            IRepositorioCuentas repositorioCuentas, IRepositorioClaseMovimientos repositorioClaseMovimientos, 
            IRepositorioCotizacionesActivos repositorioCotizacionesActivos, IRepositorioTiposActivo repositorioTiposActivo)
        {
            this.repositorioMovimientos = repositorioMovimientos;
            this.repositorioActivos = repositorioActivos;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioClaseMovimientos = repositorioClaseMovimientos;
            this.repositorioCotizacionesActivos = repositorioCotizacionesActivos;
            this.repositorioTiposActivo = repositorioTiposActivo;
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
            model.Monto = Convert.ToDecimal(model.MontoString);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await InsertarMovimiento(model);

            TempData["SuccessMessage"] = "Movimiento registrado con éxito.";
            return RedirectToAction(nameof(Crear));
        }

        public async Task InsertarMovimiento (MovimientoViewModel model)
        {
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

            if (movimiento is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            movimiento.Monto = Math.Abs(movimiento.Monto);

  

            movimiento.MontoString = movimiento.Monto.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES"));

            var cuentas = await repositorioCuentas.ObtenerPorTipo("Moneda");

            

            ViewBag.Cuentas = new SelectList(cuentas,"Id", "CuentaNombre");

            var viewModel = new MovimientoReintegroViewModel
            {
                movimiento = movimiento
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Reintegro(MovimientoReintegroViewModel reintegroVM)
        {
            var movimientoOrig = await repositorioMovimientos.ObtenerPorId(reintegroVM.movimiento.IdMovimiento);

            if (movimientoOrig is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            decimal montoNuevo = movimientoOrig.Monto + reintegroVM.montoReint;

            await repositorioMovimientos.Actualizar(movimientoOrig.IdMovimiento, montoNuevo);

            if (movimientoOrig.IdCuenta != reintegroVM.cuentaReint)
            {
                montoNuevo = - montoNuevo;

                MovimientoViewModel nuevoMovimiento = new MovimientoViewModel
                {
                    
                    //continuar armando el viewmodel con los datos correspondientes

                    TipoMovimiento = "Intercambio",
                    Fecha = reintegroVM.fechaReint,
                    IdActivo = movimientoOrig.IdActivo,
                    IdCuentaIngreso = reintegroVM.cuentaReint,
                    IdCuentaEgreso = movimientoOrig.IdCuenta,
                    Detalle = "Reintegro en otra Cuenta",
                    Monto = montoNuevo
                };

                await this.InsertarMovimiento(nuevoMovimiento);
            }

            TempData["SuccessMessage"] = "Movimiento registrado con éxito.";
            return RedirectToAction("Index");

            // return View(movimientoOrig);
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int Id)
        {
            var movimiento = await repositorioMovimientos.ObtenerPorId(Id);

            if (movimiento.TipoMovimiento == "Egreso")
            {
                movimiento.Monto = -movimiento.Monto;
            }

            movimiento.MontoString = movimiento.Monto.ToString("#,##0.00", 
                new System.Globalization.CultureInfo("es-ES"));

            if (movimiento is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            movimiento.Monto = Math.Round(movimiento.Monto, 2);
            return View(movimiento);
        }

        [HttpPost] 
        public async Task<IActionResult> Editar(Movimiento movimiento)
        {
            var movimientoExiste = await repositorioMovimientos.ObtenerPorId(movimiento.IdMovimiento);

            if (movimientoExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            movimiento.Monto = Convert.ToDecimal(movimiento.MontoString); 

            if (movimiento.TipoMovimiento == "Egreso")
            {
                movimiento.Monto = -movimiento.Monto;
            }

            if (!ModelState.IsValid)
            {
                return View(movimiento);
            }


            await repositorioMovimientos.Actualizar(movimiento.IdMovimiento, movimiento.Monto);

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> MovCrypto()
        {
            ViewBag.ActivosCrypto = await repositorioActivos.ObtenerPorTipo("CriptoMoneda");
            ViewBag.ActivosMoneda = await repositorioActivos.ObtenerPorTipo("Moneda");
            ViewBag.CuentasCrypto = await repositorioCuentas.ObtenerPorTipo("Criptomoneda");
            ViewBag.CuentasMoneda = await repositorioCuentas.ObtenerPorTipo("Moneda");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MovCrypto(InvCryptoViewModel viewModel)
        {
            
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var idMovimiento = await repositorioMovimientos.ObtenerIdMaximo() + 1;

            if (viewModel.TipoMovimiento == "Ingreso")
            {
                decimal cotiz = Convert.ToDecimal(viewModel.CotizacionIng);

                Activo activoIng = await repositorioActivos.ObtenerPorId((int)viewModel.IdActivoIng);
                Cuenta cuentaIng = await repositorioCuentas.ObtenerPorId((int)viewModel.IdCuentaIng);

                if (activoIng is null || cuentaIng is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                Movimiento movimientoIng = new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = (int)viewModel.IdCuentaIng,
                    IdActivo = (int)viewModel.IdActivoIng,
                    TipoMovimiento = "Ingreso",
                    IdClaseMovimiento = null,
                    Comentario = viewModel.TipoComercio,
                    Monto = (decimal)viewModel.CantidadIng,
                    Fecha = (DateTime)viewModel.Fecha,
                    PrecioCotiz = cotiz,
                    ActivoNombre = activoIng.ActivoNombre
                };

                await repositorioMovimientos.InsertarMovimiento(movimientoIng);

                if (viewModel.TipoComercio == "Comercio Fiat/Cripto")
                {
                    Activo activoEgr = await repositorioActivos.ObtenerPorId((int)viewModel.IdActivoEgr);
                    Cuenta cuentaEgr = await repositorioCuentas.ObtenerPorId((int)viewModel.IdCuentaEgr);
                    ClaseMovimiento claseInversion = await repositorioClaseMovimientos.ObtenerPorDescripcion("Inversiones");

                    if (activoIng is null || cuentaIng is null || claseInversion is null) 
                    {
                        return RedirectToAction("NoEncontrado", "Home");
                    }
                    string tipoActivo;
                    if(activoEgr.ActivoNombre == "Peso Argentino")
                    {
                        tipoActivo = "BLUE";
                    }
                    else
                    {
                        tipoActivo = null;
                    }

                    CotizacionActivo cotizacion = await repositorioCotizacionesActivos.GetUltimaCotizPorMoneda((int)viewModel.IdActivoEgr,tipoActivo);


                    Movimiento movimientoEgr = new Movimiento
                    {
                        IdMovimiento = idMovimiento + 1,
                        IdCuenta = (int)viewModel.IdCuentaEgr,
                        IdActivo = (int)viewModel.IdActivoEgr,
                        TipoMovimiento = "Egreso",
                        IdClaseMovimiento = claseInversion.Id,
                        Comentario = viewModel.TipoComercio,
                        Monto = (decimal)viewModel.CantidadEgr,
                        Fecha = viewModel.Fecha,
                        PrecioCotiz = Convert.ToDecimal(cotizacion.Valor),
                        ActivoNombre = activoEgr.ActivoNombre
                    };

                    await repositorioMovimientos.InsertarMovimiento(movimientoEgr);
                }
            }
            else if (viewModel.TipoMovimiento == "Egreso")
            {

                Activo activoEgr = await repositorioActivos.ObtenerPorId((int)viewModel.IdActivoEgr);
                Cuenta cuentaEgr = await repositorioCuentas.ObtenerPorId((int)viewModel.IdCuentaEgr);

                if (activoEgr is null || cuentaEgr is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                decimal cotiz =  Convert.ToDecimal(viewModel.CotizacionEgr);

                Movimiento movimientoEgr = new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = (int)viewModel.IdCuentaEgr,
                    IdActivo = (int)viewModel.IdActivoEgr,
                    TipoMovimiento = "Egreso",
                    IdClaseMovimiento = null,
                    Comentario = viewModel.TipoComercio,
                    Monto = (decimal)viewModel.CantidadEgr,
                    Fecha = viewModel.Fecha,
                    PrecioCotiz = cotiz,
                    ActivoNombre = activoEgr.ActivoNombre
                };

                await repositorioMovimientos.InsertarMovimiento(movimientoEgr);

                if(viewModel.TipoComercio == "Comercio Fiat/Cripto")
                {
                    Activo activoIng = await repositorioActivos.ObtenerPorId((int)viewModel.IdActivoIng);
                    Cuenta cuentaIng = await repositorioCuentas.ObtenerPorId((int)viewModel.IdCuentaIng);
                    ClaseMovimiento claseInversion = await repositorioClaseMovimientos.ObtenerPorDescripcion("Ingreso Inversiones");

                    if (activoIng is null || cuentaIng is null || claseInversion is null)
                    {
                        return RedirectToAction("NoEncontrado", "Home");
                    }

                    string tipoActivo;
                    if (activoIng.ActivoNombre == "Peso Argentino")
                    {
                        tipoActivo = "BLUE";
                    }
                    else
                    {
                        tipoActivo = null;
                    }


                    CotizacionActivo cotizacion = await repositorioCotizacionesActivos.GetUltimaCotizPorMoneda((int)viewModel.IdActivoIng, tipoActivo);


                    Movimiento movimientoIng = new Movimiento
                    {
                        IdMovimiento = idMovimiento + 1,
                        IdCuenta = (int)viewModel.IdCuentaIng,
                        IdActivo = (int)viewModel.IdActivoIng,
                        TipoMovimiento = "Ingreso",
                        IdClaseMovimiento = claseInversion.Id,
                        Comentario = viewModel.TipoComercio,
                        Monto = (decimal)viewModel.CantidadIng,
                        Fecha = viewModel.Fecha,
                        PrecioCotiz = Convert.ToDecimal(cotizacion.Valor),
                        ActivoNombre = activoIng.ActivoNombre
                    };

                    await repositorioMovimientos.InsertarMovimiento(movimientoIng);
                }

            }
            else if (viewModel.TipoMovimiento == "Intercambio" && viewModel.TipoComercio == "Trading")
            {
                Activo activoIng = await repositorioActivos.ObtenerPorId((int)viewModel.IdActivoIng);
                Cuenta cuentaIng = await repositorioCuentas.ObtenerPorId((int)viewModel.IdCuentaIng);
                Activo activoEgr = await repositorioActivos.ObtenerPorId((int)viewModel.IdActivoEgr);
                Cuenta cuentaEgr = await repositorioCuentas.ObtenerPorId((int)viewModel.IdCuentaEgr);

                if (activoEgr is null || cuentaEgr is null || activoIng is null || cuentaIng is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                decimal cotizIng =  Convert.ToDecimal(viewModel.CotizacionIng);

                Movimiento movimientoIng = new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = (int)viewModel.IdCuentaIng,
                    IdActivo = (int)viewModel.IdActivoIng,
                    TipoMovimiento = "Ingreso",
                    IdClaseMovimiento = null,
                    Comentario = viewModel.TipoComercio,
                    Monto = (decimal)viewModel.CantidadIng,
                    Fecha = viewModel.Fecha,
                    PrecioCotiz = cotizIng,


                };

                await repositorioMovimientos.InsertarMovimiento(movimientoIng);

                decimal cotizEgr =  Convert.ToDecimal(viewModel.CotizacionEgr);

                Movimiento movimientoEgr = new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = (int)viewModel.IdCuentaEgr,
                    IdActivo = (int)viewModel.IdActivoEgr,
                    TipoMovimiento = "Egreso",
                    IdClaseMovimiento = null,
                    Comentario = viewModel.TipoComercio,
                    Monto = (decimal)viewModel.CantidadEgr,
                    Fecha = viewModel.Fecha,
                    PrecioCotiz = cotizEgr
                };

                await repositorioMovimientos.InsertarMovimiento(movimientoEgr);
            }

            TempData["SuccessMessage"] = "Movimiento registrado con éxito.";
            return RedirectToAction(nameof(MovCrypto));

        }

        public async Task<IActionResult> MovBolsa()
        {
            ViewBag.TiposActivo = await repositorioTiposActivo.ObtenerBolsa();
            ViewBag.ActivosAccion = await repositorioActivos.ObtenerPorTipo("Accion Argentina");
            ViewBag.ActivosBonos = await repositorioActivos.ObtenerPorTipo("Bonos");
            ViewBag.ActivosCEDEAR = await repositorioActivos.ObtenerPorTipo("CEDEAR");
            ViewBag.ActivosMoneda = await repositorioActivos.ObtenerPorTipo("Moneda");
            ViewBag.ActivosFCI = await repositorioActivos.ObtenerPorTipo("FCI");
            ViewBag.CuentasBolsa = await repositorioCuentas.ObtenerPorTipo("FCI");

            return View();
        }
    }
}
