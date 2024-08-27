namespace AppFinanzasWeb.ViewModels
{
    public class IntMonedasViewModel
    {
        public DateTime Fecha { get; set; }
        public int? IdActivoEgr { get; set; }
        public int? IdCuentaEgr { get; set; }
        public decimal? CantidadEgr { get; set; }
        public int? IdActivoIng { get; set; }
        public int? IdCuentaIng { get; set; }
        public decimal? CantidadIng { get; set; }
    }
}
