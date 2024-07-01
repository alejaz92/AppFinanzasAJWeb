using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:50)]
        public string Nombre { get; set; }  
    }
}
