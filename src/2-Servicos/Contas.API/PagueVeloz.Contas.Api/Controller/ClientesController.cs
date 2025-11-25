using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Contas.Aplicacao.Comandos;
using PagueVeloz.Contas.Dominio.Request;
using System.Net.Mime;

namespace PagueVeloz.Contas.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")] 
    [Produces(MediaTypeNames.Application.Json)]
    public class ClientesController : ControllerBase
    {
        private readonly ISender _mediator;

        public ClientesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CriarClienteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarCliente([FromBody] CriarClienteRequest request)
        {
            var command = new CriarClienteCommand(request.ClienteId, request.Nome);

            try
            {
                CriarClienteResponse response = await _mediator.Send(command);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}