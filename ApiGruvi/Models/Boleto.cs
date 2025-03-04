using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiGruvi.Models
{
    public class Boleto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [ForeignKey("Viaje")]
        [Column("viaje_id")]
        public int Viaje_Id { get; set; }

        public DateTime Fecha_Compra { get; set; }

        // Relación con Usuario
        public Usuarios Usuario { get; set; }

        // Relación con Viaje
        public Viaje Viaje { get; set; }

        public Boleto()
        {
            Fecha_Compra = DateTime.Now;
        }
    }
}