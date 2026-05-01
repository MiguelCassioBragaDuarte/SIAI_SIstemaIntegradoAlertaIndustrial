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
        // Propriedades para a Lista
        public ObservableCollection<AlertaDTO> ListaAlertas { get; set; }

        // Propriedades para o Cadastro Manual (Abas)
        private string _novaMensagem;
        public string NovaMensagem
        {
            get => _novaMensagem;
            set { _novaMensagem = value; OnPropertyChanged(); }
        }

        private int _novaGravidade = 1; // Padrão: Informativo
        public int NovaGravidade
        {
            get => _novaGravidade;
            set { _novaGravidade = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand CarregarAlertasCommand { get; }
        public ICommand SalvarManualCommand { get; }

        public MainViewModel()
        {
            ListaAlertas = new ObservableCollection<AlertaDTO>();

            // Inicialização dos Comandos
            CarregarAlertasCommand = new RelayCommand(CarregarAlertas);
            SalvarManualCommand = new RelayCommand(ExecutarSalvamentoManual);

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
                    // PERSISTÊNCIA LOCAL: Atualiza o cache SQLite
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
                // FALLBACK: Busca no cache local se a API falhar
                TentarCarregarDoCacheLocal(ex.Message);
            }
        }

        private async void ExecutarSalvamentoManual()
        {
            if (string.IsNullOrWhiteSpace(NovaMensagem))
            {
                MessageBox.Show("Por favor, digite uma mensagem para o alerta.");
                return;
            }

            var novoAlerta = new AlertaDTO
            {
                Mensagem = "[MANUAL] " + NovaMensagem,
                NivelGravidade = NovaGravidade,
                DataHora = DateTime.Now,
                Equipamento = "PAINEL_WPF"
            };

            try
            {
                // 1. PERSISTÊNCIA NA API (Banco Central SQL Server)
                using var http = new HttpClient();
                var response = await http.PostAsJsonAsync("https://localhost:7127/api/Alerta/manual", novoAlerta);

                if (response.IsSuccessStatusCode)
                {
                    // 2. PERSISTÊNCIA DIRETA NO CACHE LOCAL (WPF SQLite)
                    using (var db = new InterfaceContext())
                    {
                        db.Database.EnsureCreated();
                        db.AlertasCache.Add(novoAlerta);
                        await db.SaveChangesAsync();
                    }

                    MessageBox.Show("Alerta persistido com sucesso na API e no Cache Local!");

                    // Limpa os campos da aba de gerenciamento
                    NovaMensagem = string.Empty;
                    CarregarAlertas(); // Recarrega a lista principal
                }
                else
                {
                    MessageBox.Show("A API retornou um erro ao tentar salvar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro de conexão: {ex.Message}. O dado não pôde ser sincronizado.");
            }
        }

        private void TentarCarregarDoCacheLocal(string erroOriginal)
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
                        MessageBox.Show("Aviso: API Offline. Exibindo dados do cache local (SQLite).");
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Erro de conexão e cache vazio: {erroOriginal}");
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