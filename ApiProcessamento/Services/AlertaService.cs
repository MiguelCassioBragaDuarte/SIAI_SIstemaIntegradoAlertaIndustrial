using ApiProcessamento.Models;
using ApiProcessamento.Repositories.Interfaces;


namespace ApiProcessamento.Services
{
    public interface IAlertaService
    {
        Task<IEnumerable<Alerta>> ObterTodosAlertasAsync();
        Task ProcessarLeituraAsync(LeituraSensor leitura);
    }

    // 2. Implementação da Classe
    public class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository _repository;

        // O Service usa o Repository para acessar o banco
        public AlertaService(IAlertaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Alerta>> ObterTodosAlertasAsync()
        {
            // Busca os alertas ordenados pelos mais críticos (Nível 3)
            var alertas = await _repository.ListarTodosAsync();
            return alertas.OrderByDescending(a => a.NivelGravidade);
        }

        public async Task ProcessarLeituraAsync(LeituraSensor leitura)
        {
            // Primeiro, salvamos a leitura no banco de dados
            await _repository.SalvarLeituraAsync(leitura);

            // Regra de Negócio: Se o valor for crítico, gera um alerta automático
            if (leitura.Valor > 90)
            {
                var novoAlerta = new Alerta
                {
                    Mensagem = $"PERIGO: {leitura.SensorNome} atingiu {leitura.Valor}{leitura.UnidadeMedida}!",
                    NivelGravidade = 3, // Crítico
                    LeituraSensorId = leitura.Id,
                    Resolvido = false
                };

                await _repository.CriarAlertaAsync(novoAlerta);
            }
        }
    }
}