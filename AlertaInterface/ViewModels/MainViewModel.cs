using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using AlertaInterface.Commands;
using Shared.DTOs;

namespace AlertaInterface.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        
        public ObservableCollection<AlertaDTO> ListaAlertas { get; set; }

        public ICommand CarregarAlertasCommand { get; }

        public MainViewModel()
        {
            
            ListaAlertas = new ObservableCollection<AlertaDTO>();


            CarregarAlertasCommand = new RelayCommand(CarregarAlertas);

            CarregarAlertas();
        }

        private async void CarregarAlertas()
        {
            try
            {
                using var http = new HttpClient();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

              
                var dados = await http.GetFromJsonAsync<List<AlertaDTO>>(
                    "https://localhost:7127/api/Alerta", options);

                if (dados != null)
                {
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
                MessageBox.Show($"Certifique-se de que a API está rodando na porta 7127.\nErro: {ex.Message}");
            }
        }
    }
}