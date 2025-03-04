using ApiGruvi.Data;
using ApiGruvi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ApiGruvi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViajeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ViajeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // destinos disponibles
        [HttpGet("destinos")]
        public async Task<IActionResult> GetDestinos()
        {
            var destinos = await _context.Destinos.ToListAsync();
            return Ok(destinos);
        }

        // Viajes disponibles por lugar de salida y destino
        [HttpGet("viajes/{origenId}")]
        public async Task<IActionResult> GetViajes(int origenId)
        {
            var origen = await _context.Destinos.FindAsync(origenId);
            if (origen == null)
            {
                return BadRequest(new { message = "Lugar de salida no encontrado." });
            }

            var viajes = await _context.Viajes
                .Where(v => v.DestinoId != origenId)
                .Include(v => v.Destino) 
                .Select(v => new
                {
                    v.Id,
                    v.FechaSalida,
                    v.Precio,
                    v.AsientosDisponibles 
                })
                .ToListAsync();

            return Ok(viajes);
        }

        // Comprar boleto
        [HttpPost("comprar")]
        public async Task<IActionResult> ComprarBoleto([FromBody] ComprarBoletoRequest request)
        {
            // Verificar que el viaje existe
            var viaje = await _context.Viajes
                .Include(v => v.Destino)
                .FirstOrDefaultAsync(v => v.Id == request.ViajeId);
            if (viaje == null)
            {
                return BadRequest(new { message = "Viaje no encontrado." });
            }

            // Verificar que hay asientos disponibles
            if (viaje.AsientosDisponibles <= 0)
            {
                return BadRequest(new { message = "No hay asientos disponibles." });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Verifica los asientos
                    var updatedViaje = await _context.Viajes
                        .Where(v => v.Id == request.ViajeId && v.AsientosDisponibles > 0) 
                        .FirstOrDefaultAsync();

                    if (updatedViaje == null)
                    {
                        return BadRequest(new { message = "No hay asientos disponibles en este viaje." });
                    }

                    // Crear el boleto
                    var boleto = new Boleto
                    {
                        UsuarioId = request.UsuarioId,
                        ViajeId = request.ViajeId,
                        LugarAbordaje = request.LugarAbordaje,
                        FechaCompra = DateTime.Now
                    };

                    _context.Boletos.Add(boleto);

                    
                    updatedViaje.AsientosDisponibles--;

                   
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new
                    {
                        message = "Boleto comprado con éxito.",
                        boleto = new
                        {
                            boleto.Id,
                            boleto.UsuarioId,
                            boleto.ViajeId,
                            boleto.LugarAbordaje,
                            boleto.FechaCompra
                        }
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { message = "Hubo un error al procesar la compra del boleto.", error = ex.Message });
                }
            }
        }

        public class ComprarBoletoRequest
        {
            public int UsuarioId { get; set; }
            public int ViajeId { get; set; }
            public string LugarAbordaje { get; set; } = string.Empty;
        }
    }
}
