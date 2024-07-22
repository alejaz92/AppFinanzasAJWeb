using AppFinanzasWeb.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models
{
    public class ClaseMovimiento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 60)]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteClaseMovimiento", controller: "ClaseMovimiento")]
        [Display(Name = "Descripción")]
        public string ClaseMovimientoNombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 8)]
        [PrimeraLetraMayuscula]
        public string IngEgr {  get; set; }
        public ICollection<ClaseMovimiento> ClaseMovimientos { get; set; }
    }
}
