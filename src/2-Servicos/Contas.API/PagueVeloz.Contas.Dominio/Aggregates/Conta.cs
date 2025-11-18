using PagueVeloz.Contas.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Dominio.Aggregates
{
    ///<summary>
    ///agregado Conta.
    /// </summary>
    public class Conta
    {
        ///<summary>
        ///id unico da conta.
        /// </summary>
        public string Id { get; private set; } = string.Empty;

        ///<summary>
        ///chave estrangeira para o agregado cliente.
        ///</summary>
        public string ClienteId { get; private set; } = string.Empty;

        ///<summary>
        ///limite de credito em centavos.
        ///</summary>
        public long LimiteDeCredito { get; private set; }

        ///<summary>
        ///status atual da conta.
        ///</summary>
        public StatusConta Status { get; private set; }

        //construtor privado
        private Conta() { }

        ///<summary>
        ///metodo de fabrica para criar uma nova conta.
        ///garante que as regras sejam aplicadas na criacao.
        ///</summary>
        public static Conta Criar(string clienteId, long limiteDeCredito)
        {
            //Guard Clauses | validacoes de dominio)
            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException("A conta deve pertencer a um cliente.", nameof(clienteId));

            if (limiteDeCredito < 0)
                throw new ArgumentException("O limite de crédito não pode ser negativo.", nameof(limiteDeCredito));

            var conta = new Conta
            {
                //geramos o id aqui para garantir que ele exista antes de salvar.
                Id = $"ACC-{Guid.NewGuid():N}",
                ClienteId = clienteId,
                LimiteDeCredito = limiteDeCredito,
                Status = StatusConta.Active //nova conta sempre comeca ativada
            };

            return conta;
        }

        ///<summary>
        ///metodo de negocio para alterar o limite de credito.
        ///</summary>
        public void AlterarLimiteDeCredito(long novoLimite)
        {
            if (novoLimite < 0)
                throw new ArgumentException("O limite de crédito não pode ser negativo.", nameof(novoLimite));

            LimiteDeCredito = novoLimite;
            //this.AddDomainEvent(new LimiteAlteradoDomainEvent(this.Id, novoLimite));
        }

        ///<summary>
        ///metodo de negocio para bloquear a conta.
        ///</summary>
        public void Bloquear()
        {
            if (Status == StatusConta.Blocked) return;
            Status = StatusConta.Blocked;
        }
    }
}