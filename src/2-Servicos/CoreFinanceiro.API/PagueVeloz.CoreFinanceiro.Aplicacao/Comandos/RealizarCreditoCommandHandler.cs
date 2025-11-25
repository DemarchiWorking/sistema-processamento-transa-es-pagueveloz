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
    public class RealizarCreditoCommandHandler : IConsumer<RealizarCreditoCommand>
    {
        private readonly IContaRepository _contaRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarCreditoCommandHandler(IContaRepository contaRepository, IPublishEndpoint publishEndpoint, IUnitOfWork unitOfWork)
        {
            _contaRepository = contaRepository;
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<RealizarCreditoCommand> context)
        {
            var comando = context.Message;
            Conta conta = null;
            ContaCreditoRealizadoDomainEvent eventoCredito = null;

            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            try
            {
                conta = await _contaRepository.GetByIdAsync(comando.AccountId, context.CancellationToken);

                if (conta == null)
                {
                    throw new DomainException($"Conta {comando.AccountId} não encontrada.");
                }

                conta.Credit(comando.Amount, comando.ReferenceId);

                _contaRepository.Update(conta);

                eventoCredito = conta.DomainEvents
                                    .OfType<ContaCreditoRealizadoDomainEvent>()
                                    .FirstOrDefault();

                if (eventoCredito == null)
                {
                    throw new InvalidOperationException("O Aggregate Root não lançou o evento de crédito esperado.");
                }

                await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

                conta.ClearDomainEvents();

                await _publishEndpoint.Publish(eventoCredito);
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

        private async Task PublicarFalha(RealizarCreditoCommand comando, string message)
        {
            await _publishEndpoint.Publish(new OperacaoFinanceiraFalhouEvent(
                AccountId: comando.AccountId,
                ReferenceId: comando.ReferenceId,
                MensagemErro: message,
                Operacao: "Credit",
                Valor: comando.Amount,
                Currency: comando.Currency,
                Metadata: null,
                Timestamp: DateTime.UtcNow
            ));
        }
    }
}