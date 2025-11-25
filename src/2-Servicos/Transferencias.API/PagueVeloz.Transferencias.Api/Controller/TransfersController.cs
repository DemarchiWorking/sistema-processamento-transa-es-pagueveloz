using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Eventos.Transferencias;

namespace PagueVeloz.Transferencias.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransfersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public TransfersController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarTransferencia([FromBody] TransferenciaRequest request)
        {
            var sagaId = Guid.NewGuid();

            await _publishEndpoint.Publish(new TransferenciaSolicitadaEvent(
                sagaId,
                request.ContaOrigem,
                request.ContaDestino,
                request.Valor,
                "",
                "",
                ""
            ));

            return Accepted(new { TransferenciaId = sagaId, Status = "Pending" });
        }
    }

    public record TransferenciaRequest(string ContaOrigem, string ContaDestino, long Valor);
}