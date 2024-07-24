using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class MovimientoReintegroViewModel
    {
        public Movimiento movimiento { get; set; }
        public int cuentaReint {  get; set; }
        public decimal montoReint { get; set; }
    }
}
