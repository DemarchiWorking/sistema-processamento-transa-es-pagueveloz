using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    public record ContaEstornoRealizadoDomainEvent(
        Guid TransactionId,
        string AccountId,
        long Amount,
        string Currency,
        string ReferenceId, 
        string OriginalReferenceId,
        long Balance,
        long ReservedBalance,
        long AvailableBalance,
        DateTime Timestamp
    );
}
