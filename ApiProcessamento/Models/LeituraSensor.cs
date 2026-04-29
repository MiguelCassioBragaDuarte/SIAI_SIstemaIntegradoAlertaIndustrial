namespace ApiProcessamento.Models
{
    public class LeituraSensor
    {
        public int Id { get; set; }
        public string SensorNome { get; set; }
        public double Valor { get; set; }
        public string UnidadeMedida { get; set; }
        public DateTime DataHora { get; set; }
    }
}
