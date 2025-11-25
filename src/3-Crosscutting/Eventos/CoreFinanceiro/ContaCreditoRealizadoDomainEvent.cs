using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    public record ContaCreditoRealizadoDomainEvent
    {
        public Guid TransactionId { get; init; }

        public string ReferenceId { get; init; }

        public long Valor { get; init; }

        public string AccountId { get; init; }
        
        public long SaldoDisponivelEmCentavos { get; init; }

        public long SaldoReservadoEmCentavos { get; init; }

        public long LimiteDeCreditoEmCentavos { get; init; }

        public string Currency { get; init; }
        public DateTime Timestamp { get; init; }

        public ContaCreditoRealizadoDomainEvent(
            Guid transactionId,
            string referenceId,
            long valor,
            string accountId,
            long saldoDisponivelEmCentavos,
            long saldoReservadoEmCentavos,
            long limiteDeCreditoEmCentavos,
            string currency,
            DateTime timestamp)
        {
            TransactionId = transactionId;
            ReferenceId = referenceId;
            Valor = valor;
            AccountId = accountId;
            SaldoDisponivelEmCentavos = saldoDisponivelEmCentavos;
            SaldoReservadoEmCentavos = saldoReservadoEmCentavos;
            LimiteDeCreditoEmCentavos = limiteDeCreditoEmCentavos;
            Currency = currency;
            Timestamp = timestamp;
        }
    }
}
