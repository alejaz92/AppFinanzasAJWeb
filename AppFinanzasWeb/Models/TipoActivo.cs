namespace AppFinanzasWeb.Models
{
    public class TipoActivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Activa { get; set; }

        // Relacion con Cuentas
        public ICollection<CuentaTipoActivo> CuentaTipoActivos { get; set; }
    }
}
