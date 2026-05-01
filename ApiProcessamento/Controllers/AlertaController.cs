using ApiProcessamento.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared;

namespace ApiProcessamento.Controllers
{
    /// <summary>
    /// API de Monitoramento e Processamento de Alertas Industriais (SIAI).
    /// </summary>
    /// <remarks>
    /// Esta controller gerencia o ciclo de vida dos alertas, desde a recepção de leituras de sensores 
    /// até a criação manual de alertas e exclusão de registros.
    /// 
    /// **Padrão de Erro Crítico (500):**
    /// 
    ///     {
    ///         "erro": "ERRO_INTERNO",
    ///         "mensagem": "Erro ao processar requisição.",
    ///         "detalhe": "Mensagem técnica da exceção"
    ///     }
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaRepository _repository;

        public AlertaController(IAlertaRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Lista todos os alertas registrados no banco central.
        /// </summary>
        /// <remarks>
        /// **Tipo de Envio:** GET (sem corpo).  
        /// **Retorno:** Lista de DTOs contendo detalhes do alerta e dados do sensor vinculado (se houver).
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AlertaDTO>>> GetAlertas()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "ERRO_INTERNO", mensagem = "Erro ao listar alertas.", detalhe = ex.Message });
            }
        }

        /// <summary>
        /// Recebe uma leitura de sensor e gera um alerta automático se o valor exceder o limite.
        /// </summary>
        /// <remarks>
        /// **Lógica de Alerta:** Se o valor lido for superior a 90, um alerta de nível 3 (Crítico) é criado automaticamente.
        /// 
        /// **Exemplo de Envio:**
        /// 
        ///     {
        ///        "sensorNome": "Caldeira 01",
        ///        "valor": 95.5,
        ///        "unidadeMedida": "°C",
        ///        "dataHora": "2026-04-30T21:00:00Z"
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostLeitura([FromBody] LeituraSensorDTO leituraDto)
        {
            try
            {
                if (leituraDto == null) return BadRequest(new { erro = "VALIDACAO", mensagem = "Leitura inválida" });

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
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "ERRO_INTERNO", mensagem = "Erro ao processar leitura.", detalhe = ex.Message });
            }
        }

        /// <summary>
        /// Registra um alerta manualmente via interface administrativa.
        /// </summary>
        /// <remarks>
        /// **Atenção:** Este alerta NÃO possui vínculo com leitura de sensor. O campo 'Id' é gerado automaticamente.
        /// 
        /// **Exemplo de Envio:**
        /// 
        ///     {
        ///        "mensagem": "Manutenção preventiva necessária na correia transportadora.",
        ///        "nivelGravidade": 1
        ///     }
        /// </remarks>
        [HttpPost("manual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostManual([FromBody] AlertaDTO alertaDto)
        {
            if (alertaDto == null) return BadRequest(new { erro = "VALIDACAO", mensagem = "Dados inválidos" });

            try
            {
                var novoAlerta = new Alerta
                {
                    Mensagem = "[MANUAL] " + alertaDto.Mensagem,
                    NivelGravidade = alertaDto.NivelGravidade,
                    Resolvido = false,
                    LeituraSensorId = null
                };

                await _repository.CriarAlertaAsync(novoAlerta);
                return Ok(new { message = "Persistido com sucesso no SQL Server!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = "ERRO_BANCO", mensagem = ex.Message, detalhe = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Exclui um alerta do banco central permanentemente.
        /// </summary>
        /// <param name="id">ID do alerta a ser removido.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var alertas = await _repository.ListarTodosAsync();
                if (!alertas.Any(a => a.Id == id))
                {
                    return NotFound(new { erro = "NAO_ENCONTRADO", mensagem = "Alerta não encontrado no banco central." });
                }

                await _repository.DeletarAlertaAsync(id);
                return Ok(new { message = $"Alerta {id} removido com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "ERRO_INTERNO", mensagem = "Erro ao deletar alerta.", detalhe = ex.Message });
            }
        }
    }
}