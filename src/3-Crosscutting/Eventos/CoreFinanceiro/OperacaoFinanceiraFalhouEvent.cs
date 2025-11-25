using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    public record OperacaoFinanceiraFalhouEvent(
        string AccountId,
        string ReferenceId,
        string MensagemErro,
        string Operacao,
        long Valor,
        string Currency,
        string? Metadata, 
        DateTime Timestamp);
}
