using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Comandos
{
    ///<summary>
    ///comando que representa a intencao de criar uma nova conta.
    ///</summary>
    public record CriarContaCommand(
        string ClientId,
        long InitialBalance,
        long CreditLimit
    ) : IRequest<CriarContaResponse>; //retorna um dto de resposta

    ///<summary>
    ///dto de resposta para o comando de criacao.
    ///</summary>
    public record CriarContaResponse(
        string AccountId,
        string ClientId,
        long CreditLimit,
        string Status,
        DateTime CreatedAt);
}