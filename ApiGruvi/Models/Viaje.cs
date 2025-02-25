using ApiGruvi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiGruvi.Models;
public class Viaje
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Destino { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
    public decimal Precio { get; set; }

    // Propiedad de navegación (si estás usando una relación con la clase Destino)
    [ForeignKey("DestinoId")]
    public int DestinoId { get; set; }

    public Destino? DestinoNavigation { get; set; }  // Propiedad de navegación hacia la clase Destino
}
