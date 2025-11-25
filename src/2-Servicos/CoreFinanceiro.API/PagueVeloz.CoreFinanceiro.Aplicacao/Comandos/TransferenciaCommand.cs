using MediatR;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
public class TransferenciaCommand : IRequest<TransacaoResponse>
    {
        public string SourceAccountId { get; set; } = string.Empty;
        public string DestinationAccountId { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Currency { get; set; } = "BRL";
        public string TransactionId { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
    }
}