using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertaInterface.Data
{
    class InterfaceContext: DbContext
    {
        public DbSet<AlertaDTO> AlertasCache { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=cache_interface.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlertaDTO>().HasKey(a => a.Id);
        }
    }
}
