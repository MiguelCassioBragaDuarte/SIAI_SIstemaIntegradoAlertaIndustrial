/// <summary>
/// PROJETO SHARED: Biblioteca de classes compartilhadas.
/// Serve como o "Contrato Único" da solução, garantindo que o Simulador, 
/// a API e a Interface WPF utilizem a mesma estrutura de dados.
/// </summary>
namespace Shared
{
    public class LeituraSensor
    {
        public int Id { get; set; }
        public string SensorNome { get; set; } 
        public double Valor { get; set; } 
        public string UnidadeMedida { get; set; }
        public DateTime DataHora { get; set; } 
    }

    public class Alerta
    {
        public int Id { get; set; }
        public int LeituraSensorId { get; set; } 
        public string Mensagem { get; set; } 
        public int NivelGravidade { get; set; } 
        public bool Resolvido { get; set; } 
        public virtual LeituraSensor Leitura { get; set; }
    }
}