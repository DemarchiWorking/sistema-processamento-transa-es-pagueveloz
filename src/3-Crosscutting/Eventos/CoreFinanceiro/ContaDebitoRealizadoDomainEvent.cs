using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    public record ContaDebitoRealizadoDomainEvent(
        Guid TransactionId,             
        string AccountId,
        long Valor,                     
        string Currency,
        string ReferenceId,             
        long SaldoDisponivelEmCentavos,
        long SaldoReservadoEmCentavos,
        long LimiteDeCreditoEmCentavos,
        DateTime Timestamp
    );
}