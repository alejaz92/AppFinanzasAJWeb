﻿using Microsoft.AspNetCore.Mvc;
using AppFinanzasWeb.Models;
using AppFinanzasWeb.Servicios;

namespace AppFinanzasWeb.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly IRepositorioTarjetas repositorioTarjetas;

        public TarjetaController(IRepositorioTarjetas repositorioTarjetas)
        {
            this.repositorioTarjetas = repositorioTarjetas;
        }

        public async Task<IActionResult> Index()
        {
            var tarjetas = await repositorioTarjetas.Obtener();
            return View(tarjetas);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Tarjeta tarjeta)
        {
            if (!ModelState.IsValid)
            {
                return View(tarjeta);
            }
            
            await repositorioTarjetas.Crear(tarjeta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int Id)
        {
            var tarjeta = await repositorioTarjetas.ObtenerPorId(Id);

            if (tarjeta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(tarjeta);
            }

            return View(tarjeta);
        }

        [HttpPost]

        public async Task<IActionResult> Editar(Tarjeta tarjeta)
        {
            var tarjetaExiste = await repositorioTarjetas.ObtenerPorId(tarjeta.Id);

            if (tarjetaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTarjetas.Actualizar(tarjeta);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            var tarjeta = await repositorioTarjetas.ObtenerPorId(id);

            if (tarjeta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            await repositorioTarjetas.Borrar(id);
            return Ok();
        }



        [HttpGet]
        public async Task<IActionResult> VerificarExisteTarjeta(string nombre)
        {
            var yaExisteCuenta = await repositorioTarjetas.Existe(nombre);

            if (yaExisteCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> checkEsUsado(int id)
        {
            var cuenta = await repositorioTarjetas.ObtenerPorId(id);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return Json(new { controlador = "Cuenta", result = await repositorioTarjetas.EsUsado(id) });
        }
    }
}
