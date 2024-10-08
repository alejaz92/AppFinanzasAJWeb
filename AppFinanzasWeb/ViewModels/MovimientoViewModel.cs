﻿using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.ViewModels
{
    public class MovimientoViewModel
    {
        [Required]
        public string TipoMovimiento { get; set; } // "Ingreso", "Egreso", "Intercambio"

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int IdActivo { get; set; } // Moneda

        public int? IdCuentaIngreso { get; set; } // Solo para Ingreso e Intercambio

        public int? IdCuentaEgreso { get; set; } // Solo para Egreso e Intercambio

        public int? IdClaseIngreso { get; set; } // Solo para Ingreso y Egreso

        public int? IdClaseEgreso { get; set; } // Solo para Ingreso y Egreso

        [Required]
        public string Detalle { get; set; }

       
        public decimal Monto { get; set; }
        [Required]
        public string MontoString { get; set; }
    }
}
