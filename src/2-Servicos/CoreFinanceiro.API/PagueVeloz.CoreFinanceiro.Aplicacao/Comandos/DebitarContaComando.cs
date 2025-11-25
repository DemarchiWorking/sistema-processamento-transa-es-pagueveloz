using MediatR;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public record DebitarContaComando : IRequest<TransacaoResultStatus>
    {
        public string AccountId { get; init; }
        public long Amount { get; init; }
        public string Currency { get; init; }
        public string ReferenceId { get; init; } 
        public Guid CommandId { get; init; } = Guid.NewGuid();

        public DebitarContaComando(string accountId, long amount, string currency, string referenceId)
        {
            AccountId = accountId;
            Amount = amount;
            Currency = currency;
            ReferenceId = referenceId;
        }
    }
}