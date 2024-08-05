using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;


namespace AppFinanzasWeb.Models
{
    public class MovTarjeta
    {
        public int IdMovimiento { get; set; }
        public int IdFecha { get; set; }
        public DateTime FechaMov {  get; set; } 
        public string Detalle { get; set; }
        public int IdTarjeta { get; set; }
        public Tarjeta Tarjeta { get; set; }
        public int IdClaseMovimiento { get; set; }
        public ClaseMovimiento ClaseMovimiento { get; }
        public int IdActivo {  get; set; }
        public Activo Activo { get; set; }
        public decimal MontoTotal { get; set; }
        public string MontoTotalString { get; set; }
        public int Cuotas {  get; set; }

        public int IdMesPrimerCuota { get; set; }
        public DateTime MesPrimerCuota { get; set; }
        public int IdMesUltimaCuota { get; set; }
        public DateTime MesUltimaCuota { get; set; }
        public string Repite {  get; set; }
        public string NombreMoneda { get; set; }
        public decimal MontoCuota { get; set; }
        public string CuotaTexto { get; set; }
        public decimal CuotaPesos { get; set; }
        public decimal ValorPesos { get; set; }
        public string TipoMov {  get; set; }
        public string NombreTarj { get; set; }
        public string UltCuotaTexto { get; set; }
        public bool Pagar {  get; set; }

    }
}
