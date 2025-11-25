using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{ 
    public record ContaEstornadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,           
        string OriginalReferenceId,  
        long ValorEstornado,
        string Currency,
        string? Metadata,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp);
}
