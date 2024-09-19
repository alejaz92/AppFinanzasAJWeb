using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Servicios;
using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppFinanzasWeb.ViewModels;
using System.Reflection;
using System.Text.Json;
using System.Globalization;


namespace AppFinanzasWeb.Controllers
{
    public class MovTarjetaController : Controller
    {
        private readonly IRepositorioMovTarjetas repositorioMovTarjetas;
        private readonly IRepositorioActivos repositorioActivos;
        private readonly IRepositorioClaseMovimientos repositorioClaseMovimientos;
        private readonly IRepositorioTarjetas repositorioTarjetas;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCotizacionesActivos repositorioCotizacionesActivos;
        private readonly IRepositorioMovimientos repositorioMovimientos;
        private readonly IRepositorioPagosTarjeta repositorioPagosTarjeta;
        public MovTarjetaController(IRepositorioMovTarjetas repositorioMovTarjetas, IRepositorioActivos repositorioActivos, 
            IRepositorioClaseMovimientos repositorioClaseMovimientos, IRepositorioTarjetas repositorioTarjetas, 
            IRepositorioCuentas repositorioCuentas, IRepositorioCotizacionesActivos repositorioCotizacionesActivos, 
            IRepositorioMovimientos repositorioMovimientos, IRepositorioPagosTarjeta repositorioPagosTarjeta)
        {
            this.repositorioMovTarjetas = repositorioMovTarjetas;
            this.repositorioActivos = repositorioActivos;
            this.repositorioClaseMovimientos = repositorioClaseMovimientos;
            this.repositorioTarjetas = repositorioTarjetas;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCotizacionesActivos = repositorioCotizacionesActivos;
            this.repositorioMovimientos = repositorioMovimientos;
            this.repositorioPagosTarjeta = repositorioPagosTarjeta;
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

        [HttpGet]
        public async Task<IActionResult> EditarRecurrente(int id)
        {
            var movTarjeta = await repositorioMovTarjetas.ObtenerPorId(id);

            if (movTarjeta is null )
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            movTarjeta.MontoTotalString = movTarjeta.MontoTotal.ToString("$ #,##0.00", new System.Globalization.CultureInfo("es-ES"));

            var viewModel = new MovTarjetaRecurrenteViewModel
            {
                MovTarjeta = movTarjeta
            };

            return View(viewModel); 
        }

        [HttpPost]
        public async Task<IActionResult> EditarRecurrente(MovTarjetaRecurrenteViewModel recurrenteVM)
        {
            MovTarjeta movOriginal = await repositorioMovTarjetas.ObtenerPorId(recurrenteVM.MovTarjeta.IdMovimiento);

            if(movOriginal is null )
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (recurrenteVM.Accion == "Cerrar")
            {
                DateTime mesCierreDate = new DateTime(recurrenteVM.FechaNueva.Value.Year, recurrenteVM.FechaNueva.Value.Month, 1);
                var mesCierre = int.Parse(mesCierreDate.ToString("yyyyMMdd"));

                await repositorioMovTarjetas.CerrarRecurente(movOriginal.IdMovimiento, mesCierre);
                TempData["SuccessMessage"] = "Movimiento cerrado con éxito.";
            }
            else
            {
                DateTime mesPrimerCuota = new DateTime(recurrenteVM.FechaNueva.Value.Year, recurrenteVM.FechaNueva.Value.Month, 1);

                if (mesPrimerCuota == movOriginal.MesPrimerCuota)
                {
                    await repositorioMovTarjetas.ActualizarRecurente(movOriginal.IdMovimiento, Convert.ToDecimal(recurrenteVM.MontoNuevoString));
                }
                else
                {
                    MovTarjeta movNuevo = new MovTarjeta
                    {
                        IdFecha = int.Parse(recurrenteVM.FechaNueva.Value.ToString("yyyyMMdd")),
                        Detalle = recurrenteVM.MovTarjeta.Detalle,
                        IdTarjeta = recurrenteVM.MovTarjeta.IdTarjeta,
                        IdClaseMovimiento = recurrenteVM.MovTarjeta.IdClaseMovimiento,
                        IdActivo = recurrenteVM.MovTarjeta.IdActivo,
                        MontoTotal = Convert.ToDecimal(recurrenteVM.MontoNuevoString),
                        Cuotas = 1,
                        IdMesPrimerCuota = int.Parse(mesPrimerCuota.ToString("yyyyMMdd")),
                        IdMesUltimaCuota = 0,
                        Repite = "SI",
                        MontoCuota = Convert.ToDecimal(recurrenteVM.MontoNuevoString)
                    };

                    var mesCierre = int.Parse(mesPrimerCuota.AddMonths(-1).ToString("yyyyMMdd"));

                    await repositorioMovTarjetas.InsertarMovimiento(movNuevo);
                    await repositorioMovTarjetas.CerrarRecurente(movOriginal.IdMovimiento, mesCierre);
                }
                
                TempData["SuccessMessage"] = "Movimiento actualizado con éxito.";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> PagoTarjeta()
        {


            var cotizacion = await repositorioCotizacionesActivos.GetCotizDolarTarjeta();

            var movimientosVM = new PagoTarjetaViewModel
            {
                Tarjetas = await repositorioTarjetas.Obtener(),
                Cuentas = await repositorioCuentas.ObtenerPorTipo("Moneda"),
                MesPago = DateTime.Now.AddMonths(-1),
                FechaPago = DateTime.Now,
                Cotizacion = cotizacion
            };
            

            return View(movimientosVM);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarGastos(int idTarjeta, DateTime mesPago)
        {
            if (idTarjeta == 0 || mesPago == default) 
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            string mesPagoString = mesPago.ToString("yyyy-MM") + "-01";

            var movimientos = await repositorioMovTarjetas.ObtenerMovimientosPago(idTarjeta, mesPagoString);

            var movsTransformados = movimientos.Select(movimiento => new
            {
                FechaMov = movimiento.FechaMov.ToString("yyyy-MM-dd"),
                tipoMov = movimiento.TipoMov,
                detalle = movimiento.Detalle,
                nombreMoneda = movimiento.NombreMoneda,
                cuotaTexto = movimiento.CuotaTexto,
                montoCuota = movimiento.MontoCuota,
                valorPesos = movimiento.ValorPesos,
                idActivo = movimiento.IdActivo,
                idClaseMovimiento = movimiento.IdClaseMovimiento

                
            });

            return Json(movsTransformados);
        }

        [HttpPost]
        public async Task<IActionResult> PagoTarjeta(PagoTarjetaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            viewModel.MovsTarjeta = JsonSerializer.Deserialize<List<MovTarjeta>>(viewModel.MovsTarjetaSerializados);

            ClaseMovimiento claseGastoTarjeta = await repositorioClaseMovimientos.ObtenerPorDescripcion("Gastos Tarjeta");
            Tarjeta tarjeta = await repositorioTarjetas.ObtenerPorId(viewModel.IdTarjeta);
            Activo activoPesos = await repositorioActivos.ObtenerPorNombre("Peso Argentino");

            var idMovimiento = await repositorioMovimientos.ObtenerIdMaximo() + 1;

            if(claseGastoTarjeta is null || tarjeta is null || activoPesos is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            foreach(MovTarjeta movTarjeta in viewModel.MovsTarjeta)
            {
                if(movTarjeta.Pagar)
                {
                    
                    Movimiento movimiento = new Movimiento
                    {
                        IdMovimiento = idMovimiento,
                        IdCuenta = (int)viewModel.IdCuenta,
                        TipoMovimiento = "Egreso",
                        IdClaseMovimiento = movTarjeta.IdClaseMovimiento,
                        Comentario = "(Tarjeta | " + movTarjeta.CuotaTexto + ") " + movTarjeta.Detalle,
                        Fecha = viewModel.MesPago,
                        

                    };

                    if (viewModel.MonedaPago == "Pesos")
                    {
                        movimiento.ActivoNombre = activoPesos.ActivoNombre;
                        movimiento.IdActivo = activoPesos.Id;
                        movimiento.Monto = - Convert.ToDecimal(movTarjeta.ValorPesosString, CultureInfo.InvariantCulture);
                        movimiento.PrecioCotiz = Convert.ToDecimal(viewModel.Cotizacion.Valor);
                    }
                    else
                    {
                        movimiento.ActivoNombre = movTarjeta.NombreMoneda;
                        movimiento.IdActivo = movTarjeta.IdActivo;
                        movimiento.Monto = -Convert.ToDecimal(movTarjeta.MontoCuotaString, CultureInfo.InvariantCulture);

                        if (movTarjeta.NombreMoneda == "Peso Argentino")
                        {

                            movimiento.PrecioCotiz = Convert.ToDecimal(viewModel.Cotizacion.Valor);
                        }
                        else
                        {

                            movimiento.PrecioCotiz = 1;
                        }
                    }
                    

                    await repositorioMovimientos.InsertarMovimiento(movimiento);
                    idMovimiento++;
                }
            }

            // gastos tarjeta

            

            if (Convert.ToDecimal(viewModel.TotalGastosString) != 0)
            {
                Movimiento movimiento = new Movimiento
                {
                    IdMovimiento = idMovimiento,
                    IdCuenta = viewModel.IdCuenta,
                    TipoMovimiento = "Egreso",
                    IdClaseMovimiento = claseGastoTarjeta.Id,
                    Comentario = "Gastos Tarjeta - " + tarjeta.Nombre,
                    Fecha = viewModel.FechaPago,
                    IdActivo = activoPesos.Id,
                    ActivoNombre = activoPesos.ActivoNombre,
                    Monto = Convert.ToDecimal(viewModel.TotalGastosString),
                    PrecioCotiz = Convert.ToDecimal(viewModel.Cotizacion.Valor)
                };

                await repositorioMovimientos.InsertarMovimiento (movimiento);
            }

            //registrar el pago

            PagoTarjeta pagoTarjeta = new PagoTarjeta
            {
                IdTarjeta = viewModel.IdTarjeta,
                FechaMes = viewModel.MesPago.ToString("yyyyMMdd")
            };

            await repositorioPagosTarjeta.InsertarPago (pagoTarjeta);

            TempData["SuccessMessage"] = "Pago Tarjeta registrado con éxito.";
            return RedirectToAction("Index");
        }
    }
}
