using ApiGruvi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGruvi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfilController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener boletos del usuario
        [HttpGet("{usuarioId}/boletos")]
        public async Task<IActionResult> ObtenerBoletos(int usuarioId)
        {
            var boletos = await _context.Boletos
                .Where(b => b.UsuarioId == usuarioId)
                .Include(b => b.Viaje) // Asegúrate de que tienes la relación
                .ToListAsync();

            if (!boletos.Any())
            {
                return NotFound(new { message = "No se encontraron boletos para este usuario." });
            }

            return Ok(boletos);
        }
    }
}
