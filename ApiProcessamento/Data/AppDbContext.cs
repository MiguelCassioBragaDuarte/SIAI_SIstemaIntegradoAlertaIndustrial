
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ApiProcessamento.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<LeituraSensor> LeiturasSensor { get; set; }
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeituraSensor>().HasKey(m => m.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
