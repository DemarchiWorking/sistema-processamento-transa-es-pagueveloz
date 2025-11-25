using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response
{
    public record TransacaoResultStatus
    {
        public Guid TransactionId { get; init; }
        public string Status { get; init; }
        public string ErrorMessage { get; init; }
        public long Balance { get; init; } 
        public long ReservedBalance { get; init; }
        public long AvailableBalance { get; init; }
        public DateTime Timestamp { get; init; }

        private TransacaoResultStatus(
      Guid transactionId,
      string status,
      string errorMessage,
      long balance,
      long reservedBalance,
      long availableBalance)
        {
            TransactionId = transactionId;
            Status = status;
            ErrorMessage = errorMessage;
            Balance = balance;
            ReservedBalance = reservedBalance;
            AvailableBalance = availableBalance;
            Timestamp = DateTime.UtcNow;
        }

        public static TransacaoResultStatus Sucesso(
      Guid transactionId,
      long availableBalance,
      long reservedBalance,
      long totalBalance)
        {
            return new TransacaoResultStatus(
              transactionId: transactionId,
              status: "success",
              errorMessage: null,
              balance: totalBalance,
              reservedBalance: reservedBalance,
              availableBalance: availableBalance
            );
        }

        public static TransacaoResultStatus Falha(string errorMessage, long availableBalance = 0, long reservedBalance = 0, long totalBalance = 0)
        {
            return new TransacaoResultStatus(
              transactionId: Guid.Empty,
              status: "failed",
              errorMessage: errorMessage,
              balance: totalBalance,
              reservedBalance: reservedBalance,
              availableBalance: availableBalance
            );
        }

        public static TransacaoResultStatus Pendente(string errorMessage)
        {
            return new TransacaoResultStatus(
        transactionId: Guid.Empty,
        status: "pending",
        errorMessage: errorMessage,
        balance: 0,
        reservedBalance: 0,
        availableBalance: 0
      );
        }
    }
}
