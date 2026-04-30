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
        public async Task<ActionResult<IEnumerable<Alerta>>> GetAlertas()
        {
            var alertas = await _repository.ListarTodosAsync();
            return Ok(alertas);
        }

        [HttpPost]
        public async Task<ActionResult> PostLeitura([FromBody] LeituraSensorDTO leituraDto)
        {
            if (leituraDto == null) return BadRequest("Leitura inválida");

            // 1. CONVERSÃO (Mapeamento manual): DTO -> Entidade
            // Isso é o que o professor quer ver: a recepção do DTO e a persistência da Entidade
            var leituraParaSalvar = new LeituraSensor
            {
                SensorNome = leituraDto.SensorNome,
                Valor = leituraDto.Valor,
                UnidadeMedida = leituraDto.UnidadeMedida,
                DataHora = leituraDto.DataHora
            };

            // 2. PERSISTÊNCIA: Passamos a Entidade para o repositório salvar no banco
            await _repository.SalvarLeituraAsync(leituraParaSalvar);

            // 3. REGRA DE NEGÓCIO: Usamos a entidade já persistida (que agora tem um Id gerado pelo banco)
            if (leituraParaSalvar.Valor > 90)
            {
                var novoAlerta = new Alerta
                {
                    LeituraSensorId = leituraParaSalvar.Id, // Usamos o Id que o banco acabou de criar
                    Mensagem = $"Alerta Crítico: {leituraParaSalvar.SensorNome} atingiu {leituraParaSalvar.Valor}{leituraParaSalvar.UnidadeMedida}!",
                    NivelGravidade = 3,
                    Resolvido = false
                };
                await _repository.CriarAlertaAsync(novoAlerta);
            }

            return Ok(new { message = "Leitura persistida com sucesso via DTO!" });
        }
    }
}