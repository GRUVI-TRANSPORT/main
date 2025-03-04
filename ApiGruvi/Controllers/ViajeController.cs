using ApiGruvi.Data;
using ApiGruvi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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

        // Obtener destinos disponibles
        [HttpGet("destinos")]
        public async Task<IActionResult> GetDestinos()
        {
            var destinos = await _context.Destinos.ToListAsync();
            return Ok(destinos);
        }

        // Obtener viajes disponibles por lugar de salida y destino
        [HttpGet("viajes/{origenId}")]
        public async Task<IActionResult> GetViajes(int origenId)
        {
            var origen = await _context.Destinos.FindAsync(origenId);
            if (origen == null)
            {
                return BadRequest(new { message = "Lugar de salida no encontrado." });
            }

            var viajes = await _context.Viajes
                .Where(v => v.Destino_Id != origenId)
                .Include(v => v.Destino_Navigation)
                .Select(v => new
                {
                    v.Id,
                    v.Fecha_Salida,
                    v.Precio,
                    v.Asientos_Disponibles
                })
                .ToListAsync();

            return Ok(viajes);
        }

        [HttpPost("DetalleCompra")]
        public async Task<IActionResult> DetalleCompra([FromBody] DetalleCompraRequest request)
        {
            // Obtener los datos del viaje
            var viaje = await _context.Viajes
                .Include(v => v.Destino_Navigation)  // Incluir destino para obtener la información completa
                .FirstOrDefaultAsync(v => v.Id == request.ViajeId);

            if (viaje == null)
            {
                return BadRequest(new { message = "Viaje no encontrado." });
            }

            // Obtener el usuario que está comprando el boleto
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == request.UsuarioId);

            if (usuario == null)
            {
                return BadRequest(new { message = "Usuario no encontrado." });
            }

            // Verificar que el viaje tiene suficientes asientos disponibles
            if (viaje.Asientos_Disponibles < request.Cantidad)
            {
                return BadRequest(new { message = "No hay suficientes asientos disponibles." });
            }

            // Obtener el precio de cada boleto
            decimal precioBoleto = viaje.Precio;

            // Calcular el total basado en la cantidad de boletos
            decimal totalCompra = precioBoleto * request.Cantidad;

            // Crear una respuesta con los detalles antes de realizar la compra
            var detallesCompra = new
            {
                Usuario = new
                {
                    usuario.Nombre,
                    usuario.Email
                },
                Viaje = new
                {
                    ViajeId = viaje.Id,
                    Destino = viaje.Destino_Navigation.Nombre,
                    FechaSalida = viaje.Fecha_Salida,
                    FechaLlegada = viaje.Fecha_Llegada,
                    PrecioBoleto = precioBoleto,
                    CantidadBoletos = request.Cantidad
                },
                TotalCompra = totalCompra
            };

            // Devolver los detalles de la compra
            return Ok(new
            {
                message = "Detalles de la compra.",
                detalles = detallesCompra
            });
        }



        // Modelo de solicitud para obtener los detalles de la compra
        public class DetalleCompraRequest
        {
            public int UsuarioId { get; set; }
            public int ViajeId { get; set; }
            public int Cantidad { get; set; } // Número de boletos
        }

    }
}