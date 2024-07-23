using AppFinanzasWeb.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models
{
    public class CotizacionActivo
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int IdActivoBase {  get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int IdActivoComp {  get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime Fecha { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Valor { get; set; }
    }
}
