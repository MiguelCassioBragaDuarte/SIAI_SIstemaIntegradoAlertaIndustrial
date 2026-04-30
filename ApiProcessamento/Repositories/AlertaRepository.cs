using ApiProcessamento.Data;
using ApiProcessamento.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ApiProcessamento.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly AppDbContext _context;

        public AlertaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alerta>> ListarTodosAsync()
        {
            
            return await _context.Alertas
                .Include(a => a.Leitura)
                .ToListAsync();
        }

        public async Task SalvarLeituraAsync(LeituraSensor leitura)
        {
            _context.LeiturasSensor.Add(leitura);
            await _context.SaveChangesAsync();
        }

        public async Task CriarAlertaAsync(Alerta alerta)
        {
            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();
        }
    }
}