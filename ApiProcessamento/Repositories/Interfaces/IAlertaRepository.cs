using Shared;

namespace ApiProcessamento.Repositories.Interfaces
{
    public interface IAlertaRepository
    {
        
        Task<IEnumerable<Alerta>> ListarTodosAsync();
        Task SalvarLeituraAsync(LeituraSensor leitura);
        Task CriarAlertaAsync(Alerta alerta);
    }
}