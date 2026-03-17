using ConsultorioApi.Data;
using ConsultorioApi.Models;
using ConsultorioApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultoriosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ViaCepService _viaCepService;

        public ConsultoriosController(AppDbContext context, ViaCepService viaCepService)
        {
            _context = context;
            _viaCepService = viaCepService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Consultorio>>> GetConsultorios()
        {
            return await _context.Consultorios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Consultorio>> GetConsultorio(int id)
        {
            var consultorio = await _context.Consultorios.FindAsync(id);

            if (consultorio == null)
                return NotFound();

            return consultorio;
        }

        [HttpPost]
        public async Task<ActionResult<Consultorio>> CriarConsultorio(Consultorio consultorio)
        {
            var consultorios = await _context.Consultorios.ToListAsync();

            if (consultorios.Any(c => c.Cep == consultorio.Cep))
                return BadRequest("Já existe um consultório com este CEP");

            var endereco = await _viaCepService.BuscarEnderecoAsync(consultorio.Cep);

            if (endereco == null)
                return BadRequest("CEP inválido");

            consultorio.Logradouro = endereco.Logradouro;
            consultorio.Bairro = endereco.Bairro;
            consultorio.Localidade = endereco.Localidade;
            consultorio.Uf = endereco.Uf;

            _context.Consultorios.Add(consultorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConsultorio), new { id = consultorio.Id }, consultorio);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarConsultorio(int id, Consultorio consultorio)
        {
            if (id != consultorio.Id)
                return BadRequest();

            var existente = await _context.Consultorios.FindAsync(id);

            if (existente == null)
                return NotFound();

            existente.Nome = consultorio.Nome;
            existente.Cep = consultorio.Cep;

            var endereco = await _viaCepService.BuscarEnderecoAsync(consultorio.Cep);

            if (endereco == null)
                return BadRequest("CEP inválido");

            existente.Logradouro = endereco.Logradouro;
            existente.Bairro = endereco.Bairro;
            existente.Localidade = endereco.Localidade;
            existente.Uf = endereco.Uf;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarConsultorio(int id)
        {
            var consultorio = await _context.Consultorios.FindAsync(id);

            if (consultorio == null)
                return NotFound();

            _context.Consultorios.Remove(consultorio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}