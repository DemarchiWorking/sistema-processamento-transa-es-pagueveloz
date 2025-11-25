using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagueVeloz.CoreFinanceiro.Aplicacao.Comandos;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Request;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace PagueVeloz.CoreFinanceiro.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransacaoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TransacaoResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 409)]
        public async Task<IActionResult> Post([FromBody] TransacaoFinanceiraRequest request)
        {
            if (string.IsNullOrEmpty(request.ReferenceId) || request.Amount <= 0)
            {
                return BadRequest(new { errors = "ReferenceId e Amount são obrigatórios." });
            }

            try
            {
                TransacaoResponse response = request.Operation.ToLowerInvariant() switch

                {
                    "transfer" => await _mediator.Send(new TransferenciaCommand
                    {
                        SourceAccountId = request.OriginAccountId,
                        DestinationAccountId = request.DestinationAccountId,
                        Amount = request.Amount,
                        TransactionId = request.ReferenceId,
                        Currency = request.Currency,
                        ReferenceId = request.ReferenceId
                    }),
                    _ => throw new ArgumentException($"Operação '{request.Operation}' não suportada."),
                };

                return Ok(response);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { status = "failed", error_message = $"Falha de Negócio: {ex.Message}" });
            }
            catch (ConflitoConcorrenciaException ex)
            {
                return StatusCode(409, new { status = "failed", error_message = $"Conflito de Concorrência: {ex.Message}" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { status = "failed", error_message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", error_message = $"Erro interno: {ex.Message}" });
            }
        }
    }
}
