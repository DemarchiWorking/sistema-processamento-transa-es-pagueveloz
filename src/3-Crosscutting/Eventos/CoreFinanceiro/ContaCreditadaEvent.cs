using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    
    public record ContaCreditadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,
        long ValorCreditado,
        string Currency,
        string? Metadata,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp
    );
}