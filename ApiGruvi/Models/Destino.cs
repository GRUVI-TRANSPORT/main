using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiGruvi.Models;
public class Destino
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Nombre { get; set; }

    [MaxLength(500)]
    public string? Descripcion { get; set; }

   
}
