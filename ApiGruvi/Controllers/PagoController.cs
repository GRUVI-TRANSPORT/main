using ApiGruvi.Data;
using ApiGruvi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
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

            // Obtener el viaje asociado al boleto
            var viaje = await _context.Viajes.FirstOrDefaultAsync(v => v.Id == boleto.Viaje_Id);
            if (viaje == null)
            {
                return BadRequest(new { message = "Viaje asociado al boleto no encontrado." });
            }

            // Verificar si el monto coincide con el precio del viaje
            if (request.Monto != viaje.Precio)
            {
                return BadRequest(new { message = "El monto no coincide con el precio del viaje. Pago rechazado." });
            }

            // ✅ Validar los 16 dígitos de la tarjeta
            if (string.IsNullOrWhiteSpace(request.Numero_Tarjeta) || request.Numero_Tarjeta.Length != 16 || !request.Numero_Tarjeta.All(char.IsDigit))
            {
                return BadRequest(new { message = "El número de tarjeta debe tener exactamente 16 dígitos numéricos." });
            }

            // ✅ Validar los 3 dígitos del CVV
            if (string.IsNullOrWhiteSpace(request.Cvv) || request.Cvv.Length != 3 || !request.Cvv.All(char.IsDigit))
            {
                return BadRequest(new { message = "El CVV debe tener exactamente 3 dígitos numéricos." });
            }

            // ✅ Validar fecha de expiración (mes/año)
            if (!DateTime.TryParseExact(request.FechaExpiracion, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaExpiracion))
            {
                return BadRequest(new { message = "El formato de la fecha de expiración es incorrecto. Usa MM/yyyy." });
            }

            if (fechaExpiracion < DateTime.Now)
            {
                return BadRequest(new { message = "La tarjeta está expirada. Verifica la fecha de expiración." });
            }

            var pago = new Pago
            {
                Usuario_Id = request.Usuario_Id,
                Boleto_Id = request.Boleto_Id,
                Monto = request.Monto,
                fecha_pago = DateTime.Now,
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
            public string Numero_Tarjeta { get; set; } = string.Empty; // 16 dígitos
            public string Cvv { get; set; } = string.Empty; // 3 dígitos
            public string FechaExpiracion { get; set; } = string.Empty; // Formato "MM/yyyy"
        }
    }
}
