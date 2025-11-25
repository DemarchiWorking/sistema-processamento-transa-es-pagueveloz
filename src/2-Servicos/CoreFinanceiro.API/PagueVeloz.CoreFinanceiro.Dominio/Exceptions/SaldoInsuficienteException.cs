using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Exceptions
{
    public class SaldoInsuficienteException : Exception
    {
        public SaldoInsuficienteException(string accountId, decimal requestedAmount, decimal availableLimit)
            : base($"Débito de {requestedAmount:N2} não permitido para a conta {accountId}. Limite disponível (saldo + crédito): {availableLimit:N2}.")
        {
        }
    }
}