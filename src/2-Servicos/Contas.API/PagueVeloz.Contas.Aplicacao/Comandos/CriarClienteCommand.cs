using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Comandos
{
    public record CriarClienteCommand(string ClienteId, string Nome) : IRequest<CriarClienteResponse>;

    public record CriarClienteResponse(string ClienteId, string Nome, DateTime CreatedAt);

}
