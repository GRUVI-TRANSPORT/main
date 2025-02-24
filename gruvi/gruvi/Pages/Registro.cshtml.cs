using gruvi.Data;
using gruvi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;

namespace gruvi.Pages
{
    public class Registro : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Registro(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegistroInput Input { get; set; } = new RegistroInput();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class RegistroInput
        {
            [Required]
            public string Nombre { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
            public string Password { get; set; } = string.Empty;

            [Required, Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmarPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("? ModelState inválido.");
                return Page();
            }

            Console.WriteLine($"? Nombre: {Input.Nombre}");
            Console.WriteLine($"? Email: {Input.Email}");
            Console.WriteLine($"? Password: {Input.Password}");

            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == Input.Email);
            if (usuarioExistente != null)
            {
                Console.WriteLine("? El usuario ya existe en la base de datos.");
                return RedirectToPage("/Account/Registro", new { error = "El correo ya está registrado." });
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.Password);

            var usuario = new Usuarios
            {
                Nombre = Input.Nombre,
                Email = Input.Email,
                Password = hashedPassword,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(usuario);

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("? Usuario guardado correctamente en la base de datos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error al guardar en la base de datos: {ex.Message}");
            }

            SuccessMessage = "Registro exitoso. ¡Ya puedes iniciar sesión!";
            return Page();

        }


    }
}
