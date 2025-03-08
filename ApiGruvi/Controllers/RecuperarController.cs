using ApiGruvi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ApiGruvi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecuperarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecuperarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restablecer contraseña
        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerRequest request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Validar que ambas contraseñas sean iguales
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Las contraseñas no coinciden." });
            }

            // Validar seguridad mínima de la contraseña
            if (request.Password.Length < 8 || !request.Password.Any(char.IsLetterOrDigit))
            {
                return BadRequest(new { message = "La contraseña debe tener al menos 8 caracteres." });
            }

            // Cifrar la nueva contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            usuario.Password = hashedPassword;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contraseña restablecida correctamente." });
        }


        public class RestablecerRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
