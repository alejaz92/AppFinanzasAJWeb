using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class MovimientosIndexViewModel
    {
        public IEnumerable<Movimiento> Movimientos { get; set; } 
        public int PaginaActual { get; set; }
        public int PaginasTotales { get; set; } 
    }
}
