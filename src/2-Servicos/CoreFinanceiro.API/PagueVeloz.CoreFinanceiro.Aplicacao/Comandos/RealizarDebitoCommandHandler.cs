using MassTransit;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.Comandos;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class RealizarDebitoCommandHandler : IConsumer<RealizarDebitoCommand>
    {
        private readonly IContaRepository _contaRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarDebitoCommandHandler(IContaRepository contaRepository, IPublishEndpoint publishEndpoint, IUnitOfWork unitOfWork)
        {
            _contaRepository = contaRepository;
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<RealizarDebitoCommand> context)
        {
            var comando = context.Message;
            Conta conta = null;
            ContaDebitoRealizadoDomainEvent eventoDebito = null;

            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);
            try
            {
                conta = await _contaRepository.GetByIdAsync(comando.AccountId, context.CancellationToken);

                if (conta == null)
                {
                    throw new DomainException($"Conta {comando.AccountId} não encontrada.");
                }

                conta.Debit(comando.Amount, comando.ReferenceId);

                _contaRepository.Update(conta);

                eventoDebito = conta.DomainEvents
                                    .OfType<ContaDebitoRealizadoDomainEvent>()
                                    .FirstOrDefault();

                if (eventoDebito == null)
                {
                    throw new InvalidOperationException("O Aggregate Root não lançou o evento de débito esperado.");
                }

                await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

                conta.ClearDomainEvents();

                await _publishEndpoint.Publish(eventoDebito);
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
                await PublicarFalha(comando, ex.Message);
            }
            catch (ConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(context.CancellationToken); 
                await PublicarFalha(comando, $"Falha de Concorrência: {ex.Message}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(context.CancellationToken); 
                await PublicarFalha(comando, $"Erro interno: {ex.Message}");
            }
        }

        private async Task PublicarFalha(RealizarDebitoCommand comando, string message)
        {
            await _publishEndpoint.Publish(new OperacaoFinanceiraFalhouEvent(
                AccountId: comando.AccountId,
                ReferenceId: comando.ReferenceId,
                MensagemErro: message,
                Operacao: "Debit",
                Valor: comando.Amount,
                Currency: comando.Currency,
                Metadata: null,
                Timestamp: DateTime.UtcNow
            ));
        }
    }
}