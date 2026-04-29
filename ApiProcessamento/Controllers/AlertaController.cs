using ApiProcessamento.Repositories;
using ApiProcessamento.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace ApiProcessamento.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Isso cria a rota /api/Alerta
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaRepository _repository;

        public AlertaController(IAlertaRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Alerta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alerta>>> GetAlertas()
        {
            var alertas = await _repository.ListarTodosAsync();
            return Ok(alertas);
        }

        // POST: api/Alerta
        // O Simulador vai enviar os dados para cá
        [HttpPost]
        public async Task<ActionResult> PostLeitura([FromBody] LeituraSensor leitura)
        {
            if (leitura == null) return BadRequest("Leitura inválida");

            // Salva a leitura no banco
            await _repository.SalvarLeituraAsync(leitura);

            // Regra de Negócio: Se o valor for maior que 90, gera um alerta automático
            if (leitura.Valor > 90)
            {
                var novoAlerta = new Alerta
                {
                    LeituraSensorId = leitura.Id,
                    Mensagem = $"Alerta Crítico: Temperatura de {leitura.Valor}°C atingida!",
                    NivelGravidade = 3, // Crítico
                    Resolvido = false,
                    Leitura = leitura
                };
                await _repository.CriarAlertaAsync(novoAlerta);
            }

            return Ok(new { message = "Leitura processada com sucesso" });
        }
    }
}