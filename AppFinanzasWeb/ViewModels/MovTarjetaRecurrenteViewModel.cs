using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class MovTarjetaRecurrenteViewModel
    {
        public MovTarjeta MovTarjeta {  get; set; }
        public string Accion { get; set; }
        public DateTime? FechaNueva { get; set; }

        public decimal? MontoNuevo { get; set; }

        public string? MontoNuevoString { get; set; }
    }
}
