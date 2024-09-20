namespace AppFinanzasWeb.ViewModels.Estadistica
{
    public class bolsaEstadisticaViewModel
    {
        public string NombreActivo { get; set; }
        public string simbolo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ValorOrigen {  get; set; }
        public decimal ValorActual {  get; set; }
    }

    public class bolsaGralEstadisticaViewModel
    {
        public string TipoActivo { get; set; }
        public decimal ValorOrigen { get; set; }
        public decimal ValorActual { get; set; }
    }
}
