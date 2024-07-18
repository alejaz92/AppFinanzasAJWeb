namespace AppFinanzasWeb.Models
{
    public class CuentaTipoActivo
    {
        public int IdCuenta { get; set; }
        public Cuenta Cuenta { get; set; }

        public int IdTipoActivo { get; set; }
        public TipoActivo TipoActivo { get; set; }  
    }
}
