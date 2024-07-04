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

            return View(tipoActivo);
        }
    }
}
