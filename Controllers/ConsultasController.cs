using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultorioApi.Data;
using ConsultorioApi.Models;

namespace ConsultorioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsultasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultas()
        {
            var consultas = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .ToListAsync();

            return Ok(consultas);
        }
         [HttpGet("{id}")]
        public async Task<ActionResult<Consulta>> GetConsultaId(int id)
        {
            var consulta = await _context.Consultas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.PacienteId == id);
            if (consulta == null)
            {
              return NotFound();
            }
            return consulta;
        }

        [HttpPost]
        public async Task<ActionResult> PostConsulta(Consulta consulta)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var paciente = await _context.Pacientes.FindAsync(consulta.PacienteId);
            if (paciente == null) return BadRequest($"Paciente com ID {consulta.PacienteId} não encontrado.");

            var medico = await _context.Medicos.FindAsync(consulta.MedicoId);
            if (medico == null) return BadRequest($"Médico com ID {consulta.MedicoId} não encontrado.");

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetConsultaId), new { id = consulta.Id }, consulta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditConsulta(int id, Consulta consulta)
        {
            if (id != consulta.Id) return BadRequest("Id não encontrado!");

            if(!ModelState.IsValid) return BadRequest(ModelState);
            
            var consultaExistente = await _context.Consultas.FindAsync(id);
            if (consultaExistente == null) return NotFound($"Consulta com ID {id} não encontrada.");

            var paciente = await _context.Pacientes.FindAsync(consulta.PacienteId);
            if (paciente == null) return BadRequest($"Paciente com ID {consulta.PacienteId} não encontrado.");

            var medico = await _context.Medicos.FindAsync(consulta.MedicoId);
            if (medico == null) return BadRequest($"Médico com ID {consulta.MedicoId} não encontrado.");

            consultaExistente.PacienteId = consulta.PacienteId;
            consultaExistente.MedicoId = consulta.MedicoId;
            consultaExistente.DataHora = consulta.DataHora;
            consultaExistente.Observacoes = consulta.Observacoes;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsulta(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);

            if (consulta == null)
                return NotFound();

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}