using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ApiGruvi.Data;
using ApiGruvi.Models;
using BCrypt.Net;

namespace ApiGruvi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class RegisterRequest
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

        public class RegisterResponse
        {
            public string Message { get; set; } = "Usuario registrado con éxito.";
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (usuarioExistente != null)
            {
                return BadRequest(new { message = "El correo ya está registrado." });
            }

            // Encriptar la contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

            // Crear usuario
            var usuario = new Usuarios
            {
                Nombre = request.Nombre,
                Email = request.Email,
                Password = hashedPassword
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new RegisterResponse());
        }
    }
}
