using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Exceptions
{
    public class ContaStatusInvalidoException : DomainException
    {
        public string AccountId { get; }
        public string StatusAtual { get; }

        public ContaStatusInvalidoException(string accountId, string statusAtual)
            : base($"A conta '{accountId}' está no status '{statusAtual}' e não pode realizar a operação.")
        {
            AccountId = accountId;
            StatusAtual = statusAtual;
        }
    }
}