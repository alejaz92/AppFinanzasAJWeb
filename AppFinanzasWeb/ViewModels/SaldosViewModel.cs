using AppFinanzasWeb.Models;
using AppFinanzasWeb.Models.DTO;

namespace AppFinanzasWeb.ViewModels
{
    public class SaldosViewModel
    {
        public string SaldoPesos { get; set; }
        public string SaldoDolares { get; set; }
        public TipoActivo? tipoActivo { get; set; }
        public Activo? activo { get; set; }
        public IEnumerable<CuentaMontoDTO>? CuentaMontos { get; set; } = new List<CuentaMontoDTO>();
    }
}
