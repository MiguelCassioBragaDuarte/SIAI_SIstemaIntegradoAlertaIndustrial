using ApiProcessamento.Data;
using ApiProcessamento.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ApiProcessamento.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly AppDbContext _context;

        // Injeção do Contexto do Banco de Dados
        public AlertaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alerta>> ListarTodosAsync()
        {
            // O .Include garante que os dados do Sensor venham junto com o Alerta
            return await _context.Alertas
                .Include(a => a.Leitura)
                .ToListAsync();
        }

        public async Task SalvarLeituraAsync(LeituraSensor leitura)
        {
            _context.Leituras.Add(leitura);
            await _context.SaveChangesAsync();
        }

        public async Task CriarAlertaAsync(Alerta alerta)
        {
            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();
        }
    }
}