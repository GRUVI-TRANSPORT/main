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
        [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener exactamente 16 dígitos.")]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "El número de tarjeta solo puede contener números.")]
        public string Numero_Tarjeta { get; set; } = string.Empty;

        public string Estado { get; set; } = "Completado";
    }
}
