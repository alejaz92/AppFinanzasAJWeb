using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AppFinanzasWeb.ViewModels
{
    public class PagoTarjetaViewModel
    {

        public List<MovTarjeta> MovsTarjeta { get; set; }

        public string MovsTarjetaSerializados { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int IdTarjeta { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public Cuenta Cuenta { get; set;}
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime MesPago { get; set;}
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime FechaPago { get; set;}  

        public string TotalPesosString { get; set;}
        public string? TotalDolaresString { get; set;}

        public string TotalGastosString { get; set;}

        public IEnumerable<Tarjeta> Tarjetas { get; set;}   
        public IEnumerable<Cuenta> Cuentas { get; set;}
        public CotizacionActivo Cotizacion {  get; set;}
    }
}
