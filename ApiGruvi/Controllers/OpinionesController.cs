using Microsoft.AspNetCore.Mvc;
using ApiGruvi.Data;
using ApiGruvi.Models;


namespace GruviApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionesController : ControllerBase
    {
        private static List<Opinion> opiniones = new List<Opinion>();

        [HttpGet("{destinosId}")]
        public IActionResult GetOpiniones(int destinosId)
        {
            var opinionesDestino = opiniones.Where(o => o.DestinosId == destinosId).ToList();
            return Ok(opinionesDestino);
        }

        [HttpPost]
        public IActionResult CrearOpinion([FromBody] Opinion opinion)
        {
            if (opinion == null)
            {
                return BadRequest("La opinión no puede estar vacía.");
            }

            // Aquí iría la lógica para guardar la opinión en la base de datos

            _context.Opiniones.Add(opinion);
            _context.SaveChanges();

            return Ok(opinion);
        }

        private readonly ApplicationDbContext _context;

        public OpinionesController(ApplicationDbContext context)
        {
            _context = context;
        }


    }

}