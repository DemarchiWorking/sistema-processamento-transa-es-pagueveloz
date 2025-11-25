using MassTransit;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.Comandos;
using PagueVeloz.Eventos.Contas;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class RealizarEstornoCommandHandler : IConsumer<RealizarEstornoCommand>
    {
        private readonly IContaRepository _contaRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarEstornoCommandHandler(IContaRepository contaRepository, IPublishEndpoint publishEndpoint, IUnitOfWork unitOfWork)
        {
            _contaRepository = contaRepository;
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<RealizarEstornoCommand> context)
        {
            var comando = context.Message;
            Conta conta = null;
            ContaEstornoRealizadoDomainEvent eventoEstorno = null;

            await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

            try
            {
                conta = await _contaRepository.GetByIdAsync(comando.AccountId, context.CancellationToken);

                if (conta == null)
                {
                    throw new DomainException($"Conta {comando.AccountId} não encontrada.");
                }

                conta.Estornar(
                    reversalReferenceId: comando.ReferenceId,
                    originalReferenceId: comando.OriginalReferenceId 
                );

                _contaRepository.Update(conta);

                eventoEstorno = conta.DomainEvents
                                        .OfType<ContaEstornoRealizadoDomainEvent>()
                                        .FirstOrDefault();

                if (eventoEstorno == null)
                {
                    throw new InvalidOperationException("O Aggregate Root não lançou o evento de estorno esperado.");
                }

                await _unitOfWork.CommitTransactionAsync(context.CancellationToken);

                conta.ClearDomainEvents();

                await _publishEndpoint.Publish(eventoEstorno);
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
                await PublicarFalha(comando, ex.Message); 
            }
            catch (ConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(context.CancellationToken);
                await PublicarFalha(comando, $"Erro interno: {ex.Message}");
            }
        }

        private async Task PublicarFalha(RealizarEstornoCommand comando, string message)
        {
            await _publishEndpoint.Publish(new OperacaoFinanceiraFalhouEvent(
                AccountId: comando.AccountId,
                ReferenceId: comando.OriginalReferenceId,
                MensagemErro: $"Falha ao realizar estorno (Compensação): {message}",
                Operacao: "REVERSAL",
                Valor: 0, 
                Currency: comando.Currency,
                Metadata: null,
                Timestamp: DateTime.UtcNow
            ));
        }
    }
}
