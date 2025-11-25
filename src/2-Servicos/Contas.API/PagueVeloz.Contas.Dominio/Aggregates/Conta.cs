using PagueVeloz.Contas.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Dominio.Aggregates
{
    public class Conta
    {
        public string Id { get; private set; } = string.Empty;

        public string ClienteId { get; private set; } = string.Empty;

        public long LimiteDeCredito { get; private set; }

        public StatusConta Status { get; private set; }

        public uint LockVersion { get; private set; } = 1;
        private Conta() { }

        public static Conta Criar(long id,string clienteId, long limiteDeCredito)
        {

            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException("A conta deve pertencer a um cliente.", nameof(clienteId));

            if (limiteDeCredito < 0)
                throw new ArgumentException("O limite de crédito não pode ser negativo.", nameof(limiteDeCredito));

            var conta = new Conta
            {
                Id = $"ACC-{id}",
                ClienteId = clienteId,
                LimiteDeCredito = limiteDeCredito,
                Status = StatusConta.Active, 
                LockVersion = 1 
            };

            return conta;
        }

        public void AlterarLimiteDeCredito(long novoLimite)
        {
            if (novoLimite < 0)
                throw new ArgumentException("O limite de crédito não pode ser negativo.", nameof(novoLimite));

            LimiteDeCredito = novoLimite;
        }
        public void Bloquear()
        {
            if (Status == StatusConta.Blocked) return;
            Status = StatusConta.Blocked;
        }
    }
}