using AppFinanzasWeb.Models;

namespace AppFinanzasWeb.ViewModels
{
    public class InvCryptoViewModel
    {
        public string TipoMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoComercio { get; set; }
        public int? IdActivoEgr { get; set; }
        public int? IdCuentaEgr { get; set; }
        public decimal? CantidadEgr { get; set; }
        public decimal? CotizacionEgr { get; set; }
        public int? IdActivoIng { get; set; }
        public int? IdCuentaIng { get; set; }
        public decimal? CantidadIng { get; set; }
        public decimal? CotizacionIng { get; set; }



    }
}
