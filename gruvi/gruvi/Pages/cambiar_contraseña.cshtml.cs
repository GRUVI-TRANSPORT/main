using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using gruvi.Data;
using gruvi.Models;

namespace gruvi.Pages
{
    [Authorize]
    public class cambiar_contraseña : PageModel
    {
        private readonly ApplicationDbContext _context;

        public cambiar_contraseña(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CambioContraseñaInputModel Input { get; set; }

        public string MensajeError { get; set; }
        public string MensajeExito { get; set; }

        public class CambioContraseñaInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string? PasswordActual { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
            [DataType(DataType.Password)]
            public string? PasswordNueva { get; set; }

            [Required]
            [Compare("PasswordNueva", ErrorMessage = "Las contraseñas no coinciden.")]
            [DataType(DataType.Password)]
            public string? PasswordConfirmar { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var usuario = await _context.Usuarios.FindAsync(int.Parse(userId));
            if (usuario == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(Input.PasswordActual, usuario.Password))
            {
                MensajeError = "La contraseña actual es incorrecta.";
                return Page();
            }

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(Input.PasswordNueva);
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            MensajeExito = "Contraseña cambiada con éxito.";
            return RedirectToPage("/Perfil");
        }
    }
}