using ConsultorioApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Models.Paciente> Pacientes { get; set; }
        public DbSet<Models.Consultorio> Consultorios { get; set; }
        public DbSet<Models.Medico> Medicos { get; set; }
    }
}