using PagueVeloz.CoreFinanceiro.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{
    public class TransacaoProcessada
    {
        public Guid Id { get; set; }

        public string ReferenceId { get; private set; }

        public string AccountId { get; private set; } 
        public string? DestinationAccountId { get; set; } 
        public string TransactionId { get; set; }
        public StatusTransacao Status { get; private set; }


        public string? MessageType { get; set; }

        public DateTime ProcessedAt { get; set; }
        public DateTime Timestamp { get; private set; }
        public string? Details { get; private set; }


        private TransacaoProcessada() { }

        public TransacaoProcessada(string referenceId, string accountId, string transactionId, StatusTransacao status,string destinationAccountId = null, string details = null)
        {
            if (string.IsNullOrWhiteSpace(referenceId))
                throw new ArgumentException("ReferenceId é obrigatório para idempotência.", nameof(referenceId));

            ReferenceId = referenceId;
            AccountId = accountId;
            DestinationAccountId = destinationAccountId;
            TransactionId = transactionId;
            Status = status;
            Timestamp = DateTime.UtcNow;
            Details = details;
        }
        
        public TransacaoProcessada(string referenceId, string accountId, string transactionId, string destinationAccountId, string details = null)
        {
            if (string.IsNullOrWhiteSpace(referenceId))
                throw new ArgumentException("ReferenceId é obrigatório para idempotência.", nameof(referenceId));

            ReferenceId = referenceId;
            Timestamp = DateTime.UtcNow;
            Status = StatusTransacao.Init;
            Details = "Processamento iniciado.";
            TransactionId = referenceId;
            AccountId = accountId;
            DestinationAccountId = destinationAccountId;
            Details = details;
        }
    }
}