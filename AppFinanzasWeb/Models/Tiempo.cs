using AppFinanzasWeb.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AppFinanzasWeb.Models

{
    public class Tiempo
    {
        public int IdFecha { get; set; }
        public DateTime TiempoFecha { get; set; }
        public int Anio { get; set; }   
        public int Mes {  get; set; }
        public int Dia { get; set; }
        public string MesNombre {  get; set; }
        public string DiaNombre { get;set; }
        public int Cuatrimestre { get; set; }

        public ICollection<Movimiento> Movimientos { get; set;}
    }
}
