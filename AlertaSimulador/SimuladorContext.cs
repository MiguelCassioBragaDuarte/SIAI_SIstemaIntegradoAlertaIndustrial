using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertaSimulador
{
    internal class SimuladorContext : DbContext
    {
        public DbSet<LeituraSensorDTO> HistoricoLeituras { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=simulador.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeituraSensorDTO>().HasKey(l => l.DataHora);
        }
    }
}
