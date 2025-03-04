namespace ApiGruvi.Models
{
    public class Opinion
    {
        public int Id { get; set; }
        public int UsuariosId { get; set; }
        public int DestinosId { get; set; }
        public string? Comentario { get; set; }
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
    }

}
