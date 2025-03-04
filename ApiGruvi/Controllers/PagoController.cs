using ApiGruvi.Data;
using ApiGruvi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ApiGruvi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Simular pago
        [HttpPost("realizar")]
        public async Task<IActionResult> RealizarPago([FromBody] PagoRequest request)
        {
            // Verificar si el boleto existe
            var boleto = await _context.Boletos.FirstOrDefaultAsync(b => b.Id == request.Boleto_Id);
            if (boleto == null)
            {
                return BadRequest(new { message = "Boleto no encontrado." });
            }

            // ✅ Obtener el viaje asociado al boleto
            var viaje = await _context.Viajes.FirstOrDefaultAsync(v => v.Id == boleto.Viaje_Id);
            if (viaje == null)
            {
                return BadRequest(new { message = "Viaje asociado al boleto no encontrado." });
            }

            // ✅ Verificar si el monto coincide con el precio del viaje
            if (request.Monto != viaje.Precio)
            {
                return BadRequest(new { message = "El monto no coincide con el precio del viaje. Pago rechazado." });
            }

            var pago = new Pago
            {
                Usuario_Id = request.Usuario_Id,
                Boleto_Id = request.Boleto_Id,
                Monto = request.Monto,
                fecha_pago = DateTime.Now,
                Metodo = request.Metodo,
                Estado = "Completado"
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Pago realizado con éxito.",
                pago
            });
        }

        public class PagoRequest
        {
            public int Usuario_Id { get; set; }
            public int Boleto_Id { get; set; }
            public decimal Monto { get; set; }
            public string Metodo { get; set; } = string.Empty;
        }
    }
}
