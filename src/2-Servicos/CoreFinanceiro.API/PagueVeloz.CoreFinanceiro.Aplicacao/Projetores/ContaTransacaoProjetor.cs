using MassTransit;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.Contas;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Projetores
{
    public class ContaTransacaoProjetor : IConsumer<ContaDebitoRealizadoDomainEvent>,
                                         IConsumer<ContaCreditoRealizadoDomainEvent>,
                                         IConsumer<ContaEstornoRealizadoDomainEvent>
    {
        private readonly IProjetorRepository<TransacaoProjecao> _projetorRepository;

        public ContaTransacaoProjetor(IProjetorRepository<TransacaoProjecao> projetorRepository)
        {
            _projetorRepository = projetorRepository;
        }

        public async Task Consume(ConsumeContext<ContaDebitoRealizadoDomainEvent> context)
        {
            var evento = context.Message;
            await ProjetarTransacao(
                id: evento.TransactionId,
                accountId: evento.AccountId,
                referenceId: evento.ReferenceId,
                originalReferenceId: null,
                tipo: Dominio.Entidades.TipoTransacao.Debit,
                valor: evento.Valor,
                currency: evento.Currency,
                saldo: evento.SaldoDisponivelEmCentavos, 
                timestamp: evento.Timestamp
            );
        }
        public async Task Consume(ConsumeContext<ContaCreditoRealizadoDomainEvent> context)
        {
            var evento = context.Message;
            await ProjetarTransacao(
                id: evento.TransactionId,
                accountId: evento.AccountId,
                referenceId: evento.ReferenceId,
                originalReferenceId: null,
                tipo: Dominio.Entidades.TipoTransacao.Credit,
                valor: evento.Valor,
                currency: evento.Currency,
                saldo: evento.SaldoDisponivelEmCentavos,
                timestamp: evento.Timestamp
            );
        }

        public async Task Consume(ConsumeContext<ContaEstornoRealizadoDomainEvent> context)
        {
            var evento = context.Message;

            await ProjetarTransacao(
                id: evento.TransactionId,
                accountId: evento.AccountId,
                referenceId: evento.ReferenceId,
                originalReferenceId: evento.OriginalReferenceId,
                tipo: Dominio.Entidades.TipoTransacao.Reversal,
                valor: evento.Amount,
                currency: evento.Currency,
                saldo: evento.AvailableBalance,
                timestamp: evento.Timestamp
            );
        }

        private async Task ProjetarTransacao(
            Guid id,
            string accountId,
            string referenceId,
            string originalReferenceId,
            Dominio.Entidades.TipoTransacao tipo,
            long valor,
            string currency,
            long saldo, 
            DateTime timestamp)
        {
            var projecao = new TransacaoProjecao
            {
                Id = id,
                AccountId = accountId,
                ReferenceId = referenceId,
                OriginalReferenceId = originalReferenceId,
                Tipo = tipo,
                ValorEmCentavos = valor,
                Currency = currency,
                SaldoAposTransacao = saldo,
                Timestamp = timestamp
            };

            await _projetorRepository.AddAsync(projecao);
        }
    }
}
