using ApiProcessamento.Models;

namespace ApiProcessamento.Repositories.Interfaces
{
    public interface IAlertaRepository
    {
        // Busca todos os alertas para mostrar no Dashboard (WPF)
        Task<IEnumerable<Alerta>> ListarTodosAsync();

        // Salva a leitura bruta que veio do Simulador (Console)
        Task SalvarLeituraAsync(LeituraSensor leitura);

        // Salva o alerta gerado pela regra de negócio da Service
        Task CriarAlertaAsync(Alerta alerta);
    }
}