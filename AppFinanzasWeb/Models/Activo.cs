using AppFinanzasWeb.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models
{
    public class Activo
    {
        public int Id { get; set; }
        public int IDTIPOACTIVO { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 10)]
        [LetrasMayusculas]
        public string SIMBOLO { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public bool ESPRINCIPAL { get; set; }
        public bool ESREFERENCIACOTIZ { get; set; }
        //public string TIPOACTIVO { get; set; }

    }
}
