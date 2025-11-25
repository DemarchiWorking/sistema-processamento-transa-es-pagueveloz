using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagueVeloz.CoreFinanceiro.Aplicacao.Comandos;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Request;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;

namespace PagueVeloz.CoreFinanceiro.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperacaoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OperacaoController> _logger;

        public OperacaoController(IMediator mediator, ILogger<OperacaoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOperation([FromBody] TransacaoRequest request)
        {
            if (request == null) return BadRequest("Payload inválido.");

            _logger.LogInformation("Recebida operação '{Operation}' para conta {AccountId}. Ref: {ReferenceId}",
                request.Operation, request.AccountId, request.ReferenceId);


            IRequest<TransacaoResponse>? command = request.Operation.ToLower() switch
            {
                "credit" => new ProcessarCreditoCommand
                {
                    AccountId = request.AccountId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ReferenceId = request.ReferenceId,
                    Metadata = request.Metadata
                },
                "debit" => new ProcessarDebitoCommand
                {
                    AccountId = request.AccountId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ReferenceId = request.ReferenceId,
                    Metadata = request.Metadata
                },
                "reserve" => new ProcessarReservaCommand
                {
                    AccountId = request.AccountId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ReferenceId = request.ReferenceId,
                    Metadata = request.Metadata
                },
                "capture" => new ProcessarCapturaCommand
                {
                    AccountId = request.AccountId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ReferenceId = request.ReferenceId,
                    Metadata = request.Metadata
                },
                "reversal" => new ProcessarEstornoCommand
                {
                    AccountId = request.AccountId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ReferenceId = request.ReferenceId,
                    Metadata = request.Metadata
                },
                _ => null
            };

            if (command == null)
            {
                return BadRequest(new { error_message = $"Operação '{request.Operation}' não suportada ou dados inválidos." });
            }

            try
            {
                var result = await _mediator.Send(command);

                if (result.Status == "success") return Ok(result);
                if (result.Status == "failed") return UnprocessableEntity(result); 

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro fatal no processamento da requisição.");
                return StatusCode(500, new { error_message = "Erro interno no servidor." });
            }
        }
    }
}