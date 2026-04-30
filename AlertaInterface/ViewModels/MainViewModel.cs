using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using AlertaInterface.Commands;
using AlertaInterface.Data; 
using Shared.DTOs;
using Microsoft.EntityFrameworkCore; 

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

                    using (var db = new InterfaceContext())
                    {
                        db.Database.EnsureCreated();

                        var cacheAntigo = db.AlertasCache.ToList();
                        db.AlertasCache.RemoveRange(cacheAntigo);

                        db.AlertasCache.AddRange(dados);
                        await db.SaveChangesAsync();
                    }

                    AtualizarInterface(dados);
                }
            }
            catch (Exception ex)
            {
                
                try
                {
                    using var db = new InterfaceContext();
                    if (db.Database.CanConnect())
                    {
                        var cache = db.AlertasCache.ToList();
                        if (cache.Any())
                        {
                            AtualizarInterface(cache);
                            MessageBox.Show("Aviso: API Offline. Exibindo dados do cache local (Persistência WPF).");
                        }
                    }
                }
                catch
                {
                    MessageBox.Show($"Erro de conexão e cache vazio: {ex.Message}");
                }
            }
        }

      
        private void AtualizarInterface(List<AlertaDTO> lista)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ListaAlertas.Clear();
                foreach (var item in lista)
                {
                    ListaAlertas.Add(item);
                }
            });
        }
    }
}