using AppFinanzasWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;

namespace AppFinanzasWeb.ViewModels
{
    public class PagoTarjetaViewModel
    {
        public List<MovTarjeta>? MovsTarjeta { get; set;}
        public int IdTarjeta { get; set; }
        public Cuenta Cuenta { get; set;}
        public DateTime MesPago { get; set;}
        public DateTime FechaPago { get; set;}  
        public string TotalPesosString { get; set;}
        public string? TotalDolaresString { get; set;}

        public string TotalGastosString { get; set;}

        public IEnumerable<Tarjeta> Tarjetas { get; set;}   
        public IEnumerable<Cuenta> Cuentas { get; set;}
        public CotizacionActivo Cotizacion {  get; set;}
    }
}
