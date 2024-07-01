namespace AppFinanzasWeb.Models
{
    public class Activo
    {
        public int IDACTIVO { get; set; }
        public int IDTIPOACTIVO { get; set; }
        public string SIMBOLO { get; set; }
        public string NOMBRE { get; set; }
        public bool ESPRINCIPAL { get; set; }
        public bool ESREFERENCIACOTIZ { get; set; }
        public string TIPOACTIVO { get; set; }

    }
}
