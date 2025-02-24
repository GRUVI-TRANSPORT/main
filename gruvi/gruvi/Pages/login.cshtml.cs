using gruvi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using System.Threading.Tasks;

namespace gruvi.Pages
{
    public class login : PageModel
    {
        private readonly ApplicationDbContext _context;

        public login(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public class LoginInput
        {
            [Required, EmailAddress]
            public string? Email { get; set; } = string.Empty;

            [Required]
            public string? Password { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == Input.Email);

            if (usuario != null && BCrypt.Net.BCrypt.Verify(Input.Password, usuario.Password))
            {
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioEmail", usuario.Email);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Correo o contraseña incorrectos.";
                return Page();
            }
        }
    }
}