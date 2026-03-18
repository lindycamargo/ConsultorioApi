using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultorioApi.Data;
using ConsultorioApi.Models;

namespace ConsultorioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medico>>> GetMedicos()
        {
            return await _context.Medicos.ToListAsync();
        }

        [HttpGet("{id}")] 
        public async Task<ActionResult<Medico>> GetMedico(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);

            if (medico == null)
                return NotFound();

            return medico;
        }

        [HttpPost]
        public async Task<ActionResult<Medico>> PostMedico(Medico medico)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Medicos.Add(medico);

            try
            {
                await _context.SaveChangesAsync();

                await _context.Entry(medico).Reference(m => m.Consultorio).LoadAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("IX_Medicos_Email") == true ||
                    ex.InnerException?.Message.Contains("IX_Medicos_Cpf") == true)
                {
                    return Conflict("Email ou CPF já cadastrado.");
                }

                throw;
            }

            return CreatedAtAction(nameof(GetMedico), new { id = medico.Id }, medico);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedico(int id, Paciente medico)
        {
            if (id != medico.Id)
                return BadRequest("ID da URL diferente do corpo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(medico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Medicos.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedico(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);

            if (medico == null)
                return NotFound();

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}