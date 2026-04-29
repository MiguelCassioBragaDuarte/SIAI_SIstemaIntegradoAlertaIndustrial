using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using AlertaInterface.Commands;
using Shared;

namespace AlertaInterface.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        // Usando a sua classe Alerta do Shared
        public ObservableCollection<Shared.Alerta> ListaAlertas { get; set; }

        public ICommand CarregarAlertasCommand { get; }

        public MainViewModel()
        {
            ListaAlertas = new ObservableCollection<Shared.Alerta>();
            // No seu projeto, o RelayCommand deve aceitar uma Action
            CarregarAlertasCommand = new RelayCommand(CarregarAlertas);

            CarregarAlertas(); // Carrega ao iniciar
        }

        private async void CarregarAlertas()
        {
            try
            {
                using var http = new HttpClient();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Ajuste a URL para o seu endpoint de Alertas
                var dados = await http.GetFromJsonAsync<List<Shared.Alerta>>(
                    "https://localhost:7179/api/Alerta", options);

                if (dados != null)
                {
                    // O Dispatcher garante que a UI não trave e atualize corretamente
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ListaAlertas.Clear();
                        foreach (var item in dados)
                        {
                            ListaAlertas.Add(item);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar na API: {ex.Message}");
            }
        }
    }
}