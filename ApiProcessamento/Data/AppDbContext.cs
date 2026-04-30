
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
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.Leitura)
                .WithMany()
                .HasForeignKey(a => a.LeituraSensorId);
        }
    }
}
