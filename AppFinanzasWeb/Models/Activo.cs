namespace AppFinanzasWeb.Models
{
    public class Activo
    {
        public int Id { get; set; }
        public int IDTIPOACTIVO { get; set; }
        public string SIMBOLO { get; set; }
        public string Nombre { get; set; }
        public bool ESPRINCIPAL { get; set; }
        public bool ESREFERENCIACOTIZ { get; set; }
        //public string TIPOACTIVO { get; set; }

    }
}
