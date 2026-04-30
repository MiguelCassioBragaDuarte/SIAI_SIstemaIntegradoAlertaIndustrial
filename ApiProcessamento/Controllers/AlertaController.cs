using ApiProcessamento.Repositories;
using ApiProcessamento.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared;

namespace ApiProcessamento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaRepository _repository;

        public AlertaController(IAlertaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertaDTO>>> GetAlertas()
        {
            var alertasBanco = await _repository.ListarTodosAsync();

            var listaDtos = alertasBanco.Select(a => new AlertaDTO
            {
                Id = a.Id,
                Mensagem = a.Mensagem,
                NivelGravidade = a.NivelGravidade,
                DataHora = a.Leitura != null ? a.Leitura.DataHora : DateTime.Now,
                Equipamento = a.Leitura != null ? a.Leitura.SensorNome : "Desconhecido",
                ValorLido = a.Leitura != null ? a.Leitura.Valor : 0
            }).ToList();

            return Ok(listaDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostLeitura([FromBody] LeituraSensorDTO leituraDto)
        {
            if (leituraDto == null) return BadRequest("Leitura inválida");

            var leituraParaSalvar = new LeituraSensor
            {
                SensorNome = leituraDto.SensorNome,
                Valor = leituraDto.Valor,
                UnidadeMedida = leituraDto.UnidadeMedida,
                DataHora = leituraDto.DataHora
            };

           
            await _repository.SalvarLeituraAsync(leituraParaSalvar);

            
            if (leituraParaSalvar.Valor > 90)
            {
                var novoAlerta = new Alerta
                {
                    LeituraSensorId = leituraParaSalvar.Id, 
                    Mensagem = $"Alerta Crítico: {leituraParaSalvar.SensorNome} atingiu {leituraParaSalvar.Valor}{leituraParaSalvar.UnidadeMedida}!",
                    NivelGravidade = 3,
                    Resolvido = false
                };
                await _repository.CriarAlertaAsync(novoAlerta);
            }

            return Ok(new { message = "Leitura persistida com sucesso via DTO!" });

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var alerta = await _repository.ListarTodosAsync();
            if (!alerta.Any(a => a.Id == id))
            {
                return NotFound("Alerta não encontrado no banco central.");
            }

            await _repository.DeletarAlertaAsync(id);
            return Ok(new { message = $"Alerta {id} removido com sucesso!" });
        }
    }
}