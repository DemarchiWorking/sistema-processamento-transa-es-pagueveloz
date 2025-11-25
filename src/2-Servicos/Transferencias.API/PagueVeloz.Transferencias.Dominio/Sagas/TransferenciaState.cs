using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Transferencias.Dominio.Sagas
{
    public class TransferenciaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string AccountIdOrigem { get; set; }
        public string AccountIdDestino { get; set; }
        public long Amount { get; set; }
        public string ReferenceId { get; set; }
        public string Currency { get; set; }

        public DateTime Timestamp { get; set; }
        public string? Status { get; set; } 

        public Guid? DebitRequestTokenId { get; set; } 
        public Guid? CreditRequestTokenId { get; set; }
        public string MotivoFalha { get; set; }
        public int Version { get; set; }
    }
}