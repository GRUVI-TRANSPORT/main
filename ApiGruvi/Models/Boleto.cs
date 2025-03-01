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
        public int ViajeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("lugar_abordaje")]
        public string LugarAbordaje { get; set; }

        public DateTime FechaCompra { get; set; }

        // Relación con Usuario
        public Usuarios Usuario { get; set; }

        // Relación con Viaje
        public Viaje Viaje { get; set; }

        public Boleto()
        {
            FechaCompra = DateTime.Now;
        }
    }
}
