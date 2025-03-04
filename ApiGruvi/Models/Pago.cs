using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiGruvi.Models
{
    public class Pago
    {
        public int Id { get; set; }

        [Required]
        public int Usuario_Id { get; set; }

        [Required]
        public int Boleto_Id { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser mayor o igual a 0.")]
        public decimal Monto { get; set; }

        [Required]
        public DateTime fecha_pago { get; set; }

        [Required]
        public string Metodo { get; set; } = string.Empty; // Ejemplo: "Tarjeta", "Paypal", etc.

        public string Estado { get; set; } = "Completado"; // Puede ser "Completado", "Fallido", etc.
    }
}


