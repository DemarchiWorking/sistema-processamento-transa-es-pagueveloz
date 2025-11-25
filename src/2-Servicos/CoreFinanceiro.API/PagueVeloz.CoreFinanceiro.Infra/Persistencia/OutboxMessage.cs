using Microsoft.EntityFrameworkCore;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Persistencia
{
   
    public class OutboxMessage
    {
        public Guid Id { get; private set; }

        public DateTime CreatedOnUtc { get; private set; }

        public string EventType { get; private set; }
        public string Payload { get; private set; }
        public string Type { get; private set; }

        public DateTime? ProcessedOnUtc { get; internal set; }

        public int Attempts { get; internal set; }

        public DateTime? NextAttemptDateUtc { get; internal set; }

        public string? Error { get; internal set; }

        protected OutboxMessage() { }

        public OutboxMessage(string eventType, string payload)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentNullException(nameof(eventType));
            if (string.IsNullOrWhiteSpace(payload))
                throw new ArgumentNullException(nameof(payload));

            Id = Guid.NewGuid();
            CreatedOnUtc = DateTime.UtcNow;
            EventType = eventType;
            Payload = payload;

            Attempts = 0; 
            NextAttemptDateUtc = DateTime.UtcNow; 
        }

        public void MarkAsProcessed()
        {
            ProcessedOnUtc = DateTime.UtcNow;
            NextAttemptDateUtc = DateTime.MaxValue; 
            Error = null;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Error = errorMessage;
            Attempts++;

            var delaySeconds = Math.Min(3600, Math.Pow(2, Attempts));
            NextAttemptDateUtc = DateTime.UtcNow.AddSeconds(delaySeconds);

            ProcessedOnUtc = null;
        }
    }
}
