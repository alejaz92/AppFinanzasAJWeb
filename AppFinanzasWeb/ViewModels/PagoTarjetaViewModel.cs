using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class PagoTarjetaViewModel
    {
        public IEnumerable<MovTarjeta>? MovsTarjeta { get; set;}
        public int IdTarjeta { get; set; }
        public Cuenta Cuenta { get; set;}
        public DateTime MesPago { get; set;}
        public DateTime FechaPago { get; set;}  
        public string TotalPesosString { get; set;}
        public string? TotalDolaresString { get; set;}

        public string TotalGastosString { get; set;}
    }
}
