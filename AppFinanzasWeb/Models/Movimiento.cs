using System.ComponentModel.DataAnnotations;
using AppFinanzasWeb.Validaciones;
using Microsoft.AspNetCore.Mvc;

namespace AppFinanzasWeb.Models
{
    public class Movimiento
    {
        public int IdMovimiento { get; set; }   
        public int IdCuenta { get; set; }
        public string CuentaNombre { get; set; }
        public Cuenta Cuenta { get; set; }
        public int IdActivo { get; set; }
        public string ActivoNombre { get; set; }
        public Activo Activo { get; set; }
        public int IdFecha { get; set; }
        public DateTime Fecha { get; set; } 
        public Tiempo Tiempo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string TipoMovimiento { get; set; }
        public int IdClaseMovimiento { get; set; }
        public string ClaseMovimientoNombre { get; set; }

        public ClaseMovimiento ClaseMovimiento { get; set; }

        [StringLength(maximumLength: 200)]
        public string Comentario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Monto { get; set; }  
        public decimal PrecioCotiz {  get; set; }

    }
}
