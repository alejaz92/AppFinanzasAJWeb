using AppFinanzasWeb.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:50)]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteCuenta", controller: "Cuenta")]
        public string CuentaNombre { get; set; }  

        //Relacion con tipo de activos
        public ICollection<CuentaTipoActivo> CuentaTipoActivos { get; set; }
        public ICollection<Movimiento> Movimientos { get; set; }
    }
}
