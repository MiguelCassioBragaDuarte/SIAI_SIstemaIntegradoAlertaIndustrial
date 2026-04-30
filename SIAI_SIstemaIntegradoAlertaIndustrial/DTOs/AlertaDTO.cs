using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class AlertaDTO
    {
        public int Id { get; set; }
        public string Mensagem { get; set; }
        public int NivelGravidade { get; set; }
        public DateTime DataHoraLeitura { get; set; } // Dado que vem da Leitura
    }
}
