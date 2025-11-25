using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Comandos
{
    public record CriarContaCommand(
        string ClientId,
        long InitialBalance,
        long CreditLimit
    ) : IRequest<CriarContaResponse>; 

    public record CriarContaResponse(
        string AccountId,
        string ClientId,
        long CreditLimit,
        string Status,
        DateTime CreatedAt);
}