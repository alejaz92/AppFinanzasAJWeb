using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;
using AppFinanzasWeb.Servicios;
using Microsoft.AspNetCore.Mvc.Rendering;
using AppFinanzasWeb.ViewModels;

namespace AppFinanzasWeb.Controllers
{
    public class ActivoController: Controller
    {
        private readonly IRepositorioTiposActivo repositorioTiposActivo;
        private readonly IRepositorioActivos repositorioActivos;


        public ActivoController(IRepositorioTiposActivo repositorioTiposActivo, IRepositorioActivos repositorioActivos)
        {
            this.repositorioTiposActivo = repositorioTiposActivo;
            this.repositorioActivos = repositorioActivos;
        }

        public async Task<IActionResult> Index(int? Id)
        {
            var tiposActivo = await repositorioTiposActivo.Obtener();
            ViewBag.TiposActivos = new SelectList(tiposActivo, "Id", "Nombre", Id);
            ViewBag.SelectedIdTipoActivo = Id;

            if (!Id.HasValue)
            {
                return View(new List<Activo>());
            }


            var activos = await repositorioActivos.ObtenerPorTipo(Id.Value);
            return View(activos);

        }

        [HttpGet]
        public async Task<ActionResult> Crear(int Id)
        {
            var tipoActivo = await repositorioTiposActivo.ObtenerPorId(Id);

            if (tipoActivo is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var viewModel = new ActivoCrearViewModel
            {
                Activo = new Activo
                {
                    IDTIPOACTIVO = tipoActivo.Id
                },
                TipoActivoNombre = tipoActivo.Nombre
                
                


            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ActivoCrearViewModel vmActivo)
        {
            if (!ModelState.IsValid)
            {
                return View(vmActivo);
            }

            await repositorioActivos.Crear(vmActivo.Activo);
            return RedirectToAction(nameof(Index), new {Id = vmActivo.Activo.IDTIPOACTIVO});
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int Id)
        {
            var activo = await repositorioActivos.ObtenerPorId(Id);
            if (activo is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(activo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Activo activo)
        {
            var activoExiste = await repositorioActivos.ObtenerPorId(activo.Id);
            if (activoExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(activo);
            }

            await repositorioActivos.Actualizar(activo);
            return RedirectToAction(nameof(Index), new { Id = activoExiste.IDTIPOACTIVO });
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var activo = await repositorioActivos.ObtenerPorId(id);

            if (activo is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

           

            //if (await repositorioActivos.EsUsado(id))
            //{
            //    return Json(new { success = false, message = "El activo ua esta en uso" });
            //}

            return View(activo);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarActivo(int id)
        {
            var activoExiste = await repositorioActivos.ObtenerPorId(id);

            if(activoExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioActivos.Borrar(activoExiste.Id);
            return RedirectToAction(nameof(Index), new { Id = activoExiste.IDTIPOACTIVO });
        }
        [HttpGet]
        public async Task<IActionResult> checkEsUsado(int id)
        {
            return Json(new { controlador = "Activo", result = await repositorioActivos.EsUsado(id) });
        }
    }
}
