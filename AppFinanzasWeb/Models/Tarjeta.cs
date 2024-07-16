using AppFinanzasWeb.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models
{
    public class Tarjeta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:60)]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTarjeta", controller: "Tarjeta")]
        public string Nombre { get; set; }
    }
}
