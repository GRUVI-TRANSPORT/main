using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using gruvi.Data;
using gruvi.Models;
using BCrypt.Net;

namespace gruvi.Pages
{
    public class restablecer_contraseña : PageModel
    {
        private readonly ApplicationDbContext _context;

        public restablecer_contraseña(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RestablecerContraseñaInput Input { get; set; } = new();

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                MensajeError = "El enlace de recuperación es inválido.";
                return Page();
            }

            var registro = await _context.restablecer_pwr
                .FirstOrDefaultAsync(r => r.Token == token && r.ExpiresAt > DateTime.UtcNow);

            if (registro == null)
            {
                MensajeError = "El enlace ha expirado o es inválido.";
                return Page();
            }

            Input.Token = token;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var registro = await _context.restablecer_pwr
                .FirstOrDefaultAsync(r => r.Token == Input.Token && r.ExpiresAt > DateTime.UtcNow);

            if (registro == null)
            {
                MensajeError = "El enlace de recuperación ha expirado o es inválido.";
                return Page();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == registro.Email);
            if (usuario == null)
            {
                MensajeError = "El usuario no existe.";
                return Page();
            }

            // Hashear la nueva contraseña
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(Input.Password);
            await _context.SaveChangesAsync();

            // Eliminar el registro del token
            _context.restablecer_pwr.Remove(registro);
            await _context.SaveChangesAsync();

            MensajeExito = "Tu contraseña ha sido restablecida con éxito. Ahora puedes iniciar sesión.";
            return Page();
        }

        public class RestablecerContraseñaInput
        {
            [Required]
            public string Token { get; set; } = string.Empty;

            [Required, MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}