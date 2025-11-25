using MediatR;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class ProcessarReservaCommand : IRequest<TransacaoResponse>
    {
        public string AccountId { get; init; } = string.Empty;
        public long Amount { get; init; }
        public string Currency { get; init; } = "BRL";
        public string ReferenceId { get; init; } = string.Empty;
        public string TransactionId { get; init; }
        public Dictionary<string, object>? Metadata { get; init; }
    }
}