using System;
using System.ComponentModel.DataAnnotations;

namespace gruvi.Models
{
    public class restablecer_pwr
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime Expiracion { get; set; } // Nuevo campo agregado
    }
}
