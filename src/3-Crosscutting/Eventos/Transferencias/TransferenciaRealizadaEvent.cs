using PagueVeloz.Eventos.Transferencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Transferencias
{
    public record TransferenciaRealizadaEvent
    {
        public string TransactionId { get; init; }
        public string SourceAccountId { get; init; }
        public string DestinationAccountId { get; init; }
        public string Status { get; init; } 

        public DateTime Timestamp { get; init; }
        public long Amount { get; init; }
        public string Currency { get; init; }
        public string CorrelationId { get; init; }
        public string Description { get; init; }

    }
}
