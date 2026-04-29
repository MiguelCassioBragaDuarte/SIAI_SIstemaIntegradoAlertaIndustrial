using System.Net.Http;
using System.Net.Http.Json;
using Shared;

namespace AlertaInterface.Services
{
    public class ApiService
    {
        private readonly HttpClient _http = new HttpClient();
        // DICA: Ajuste a porta conforme a sua API estiver rodando
        private const string BaseUrl = "https://localhost:7179/api/Alerta";

        public async Task<List<Alerta>> ObterAlertasAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Alerta>>(BaseUrl) ?? new List<Alerta>();
            }
            catch
            {
                return new List<Alerta>();
            }
        }
    }
}