using MassTransit;
using MediatR;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class DebitarContaComandoHandler : IRequestHandler<DebitarContaComando, TransacaoResultStatus>
    {
        private readonly IContaRepository _contaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public DebitarContaComandoHandler(
            IContaRepository contaRepository,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint)
        {
            _contaRepository = contaRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<TransacaoResultStatus> Handle(DebitarContaComando request, CancellationToken cancellationToken)
        {
            var conta = await _contaRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (conta == null)
            {
                return TransacaoResultStatus.Falha("Conta não encontrada.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var transacaoExistente = conta.Transacoes.FirstOrDefault(t => t.ReferenceId == request.ReferenceId);

                if (transacaoExistente != null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return TransacaoResultStatus.Sucesso(
                        transacaoExistente.Id,
                        conta.SaldoDisponivelEmCentavos,
                        conta.SaldoReservadoEmCentavos,
                        conta.SaldoTotal
                    );
                }

                conta.Debit(
                    request.Amount,
                    request.ReferenceId
                );

                var eventoDebito = conta.DomainEvents
                    .OfType<ContaDebitoRealizadoDomainEvent>()
                    .Last(); 

                _contaRepository.Update(conta);

                await _publishEndpoint.Publish(eventoDebito, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return TransacaoResultStatus.Sucesso(
                    eventoDebito.TransactionId,
                    conta.SaldoDisponivelEmCentavos,
                    conta.SaldoReservadoEmCentavos,
                    conta.SaldoTotal
                );
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                return TransacaoResultStatus.Falha(
                    ex.Message,
                    conta.SaldoDisponivelEmCentavos,
                    conta.SaldoReservadoEmCentavos,
                    conta.SaldoTotal
                );
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                return TransacaoResultStatus.Pendente("Falha no processamento por concorrência/infraestrutura. A operação pode estar pendente para reprocessamento.");
            }
        }
    }
}