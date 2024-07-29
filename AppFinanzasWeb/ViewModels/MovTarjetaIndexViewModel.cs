using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class MovTarjetaIndexViewModel
    {
        public IEnumerable<MovTarjeta> Movimientos { get; set; }
        public int PaginaActual { get; set; }
        public int PaginasTotales { get; set; }
    }
}
