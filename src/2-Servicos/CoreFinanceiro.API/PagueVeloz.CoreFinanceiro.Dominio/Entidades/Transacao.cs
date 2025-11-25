using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{
    public enum TipoTransacao
    {
        Credit,
        Debit,
        Reserve,
        Capture,
        Reversal,
        Release,
    }
    public class Transacao
    {
        public Guid Id { get; private set; } 
        public string ContaId { get; private set; }
        public TipoTransacao Tipo { get; private set; }
        public long Valor { get; private set; }
        public string Currency { get; private set; }
        public string ReferenceId { get; private set; } 
        public DateTime Timestamp { get; private set; }

        public string Status { get; private set; }
        public string? ErrorMessage { get; private set; } 
        public string? Metadata { get; private set; } 

        public long FinalBalanceInCents { get; private set; }
        public long FinalReservedBalanceInCents { get; private set; }
        public long FinalAvailableBalanceInCents { get; private set; }

        public bool IsReversed { get; private set; } 
        public string? ReversalReferenceId { get; private set; }

        private Transacao() { }

        public Transacao(
            string contaId,
            TipoTransacao tipo,
            long valor,
            string referenceId,
            string currency,
            string status,
            long finalBalance,
            long finalReservedBalance,
            long finalAvailableBalance,
            string? metadata = null,
            string? errorMessage = null)
        {
            Id = Guid.NewGuid();
            ContaId = contaId;
            Tipo = tipo;
            Valor = valor;
            ReferenceId = referenceId;
            Currency = currency;
            Timestamp = DateTime.UtcNow;

            Status = status;
            FinalBalanceInCents = finalBalance;
            FinalReservedBalanceInCents = finalReservedBalance;
            FinalAvailableBalanceInCents = finalAvailableBalance;
            Metadata = metadata;
            ErrorMessage = errorMessage;
        }

        public void MarcarComoEstornada(string reversalReferenceId)
        {
            if (IsReversed)
            {
                throw new DomainException($"Transação {ReferenceId} já foi estornada pela transação {ReversalReferenceId}.");
            }

            IsReversed = true;
            ReversalReferenceId = reversalReferenceId;
        }

        public void RegistrarFalha(string errorMessage)
        {
            Status = "failed";
            ErrorMessage = errorMessage;
        }
    }
}