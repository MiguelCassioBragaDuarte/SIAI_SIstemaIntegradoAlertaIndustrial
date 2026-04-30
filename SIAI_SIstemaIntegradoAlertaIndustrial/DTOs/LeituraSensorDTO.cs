using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class LeituraSensorDTO
    {
        public string SensorNome { get; set; }
        public double Valor { get; set; }
        public string UnidadeMedida { get; set; }
        public DateTime DataHora { get; set; }
    }
}
