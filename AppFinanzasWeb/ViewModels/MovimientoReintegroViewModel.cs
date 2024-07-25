using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class MovimientoReintegroViewModel
    {
        public Movimiento movimiento { get; set; }
        public int cuentaReint {  get; set; }
        public DateTime fechaReint { get; set; }
        public decimal montoReint { get; set; }
    }
}
