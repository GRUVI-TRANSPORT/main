using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ApiGruvi.Data;
using BCrypt.Net;

namespace ApiGruvi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class LoginRequest
        {
            [Required, EmailAddress]
            public string? Email { get; set; }

            [Required]
            public string? Password { get; set; }
        }

        public class LoginResponse
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Message { get; set; } = "Inicio de sesión exitoso.";
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null || string.IsNullOrEmpty(usuario.Password) || !usuario.Password.StartsWith("$2a$"))
            {
                return Unauthorized(new { message = "Correo o contraseña incorrectos." });
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
            {
                return Unauthorized(new { message = "Correo o contraseña incorrectos." });
            }

            return Ok(new LoginResponse
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Sesión cerrada correctamente." });
        }


    }
}
