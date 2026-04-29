namespace ApiProcessamento.Models
{
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
