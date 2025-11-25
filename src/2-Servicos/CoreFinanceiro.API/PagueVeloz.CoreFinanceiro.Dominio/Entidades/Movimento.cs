using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{
    public class Movimento
    {
        public string Id { get; private set; }

        public string AccountId { get; private set; }
        public Conta Account { get; private set; } 

        public TipoTransacao Tipo { get; private set; }
        public decimal Value { get; private set; }
        public string Coin { get; private set; }
        public string ReferenceId { get; private set; }

        public string TransactionId { get; private set; }

        public DateTime Timestamp { get; private set; }
        public string MetadadosJson { get; private set; }
        private Movimento() { }

        public Movimento(
            string contaId,
            TipoTransacao tipo,
            decimal valor,
            string moeda,
            string referenceId,
            string transactionId,
            string metadadosJson = null)
        {
            if (valor <= 0)
                throw new ExcecaoDominio("O valor do movimento deve ser positivo.");

            Id = Guid.NewGuid().ToString();
            AccountId = contaId;
            Tipo = tipo;
            Value = valor;
            Coin = moeda;
            ReferenceId = referenceId;
            TransactionId = transactionId;
            Timestamp = DateTime.UtcNow;
            MetadadosJson = metadadosJson;
        }
    }
}