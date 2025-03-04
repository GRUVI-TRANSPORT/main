using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiGruvi.Models
{
    public class Viaje
    {
        public int Id { get; set; }

        //[Required]
        //[StringLength(100)]
        //public string? Destino { get; set; }

        //[Required]
        //public DateTime Fecha { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Los asientos disponibles deben ser mayores o iguales a 0.")]
        public int Asientos_Disponibles { get; set; } 

        [Required]
        public DateTime Fecha_Salida { get; set; } 

        public int Destino_Id { get; set; }

        [ForeignKey("Destino_Id")]
        public Destino? Destino_Navigation { get; set; }
        public DateTime Fecha_Llegada { get; set; }

    }
}
