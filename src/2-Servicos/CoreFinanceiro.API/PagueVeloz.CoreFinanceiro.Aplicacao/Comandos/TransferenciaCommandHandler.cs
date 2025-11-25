using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.Transferencias;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class TransferenciaCommandHandler : IRequestHandler<TransferenciaCommand, TransacaoResponse>
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoProcessadaRepository _transacaoProcessadaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransferenciaCommandHandler> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IPublishEndpoint _publishEndpoint;
        //private readonly IMessageProducer _messageProducer;


        public TransferenciaCommandHandler(
            IContaRepository contaRepository,
            ITransacaoProcessadaRepository transacaoProcessadaRepository,
            IUnitOfWork unitOfWork,
            //IMessageProducer messageProducer,
            ILogger<TransferenciaCommandHandler> logger,
            IPublishEndpoint publishEndpoint)

        {
            _contaRepository = contaRepository;
            _transacaoProcessadaRepository = transacaoProcessadaRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            //_messageProducer = messageProducer; 

            _retryPolicy = Policy
                .Handle<ConflitoConcorrenciaException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(50 * Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Conflito de concorrência detectado. Tentando novamente ({RetryCount}/{MaxRetries}) após {Delay}ms.",
                            retryCount, 3, timeSpan.TotalMilliseconds);
                    }
                );
        }

        public async Task<TransacaoResponse> Handle(TransferenciaCommand request, CancellationToken cancellationToken)
        {
            if (await _transacaoProcessadaRepository.JaProcessadaAsync(request.ReferenceId, cancellationToken))
            {
                _logger.LogWarning("Transferência duplicada: {ReferenceId}", request.ReferenceId);
                return new TransacaoResponse($"DUPLICATE-{request.ReferenceId}", "success", 0, 0, 0, DateTime.UtcNow, null);
            }

            if (request.Amount <= 0) return FailResponse("Valor deve ser positivo.", request.ReferenceId);

            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _unitOfWork.BeginTransactionAsync(cancellationToken);

                    var id1 = request.SourceAccountId;
                    var id2 = request.DestinationAccountId;
                    var firstId = string.Compare(id1, id2) < 0 ? id1 : id2;
                    var secondId = firstId == id1 ? id2 : id1;

                    var conta1 = await _contaRepository.GetByIdAsync(firstId, cancellationToken);
                    var conta2 = await _contaRepository.GetByIdAsync(secondId, cancellationToken);

                    var origem = conta1?.AccountId == request.SourceAccountId ? conta1 : conta2;
                    var destino = conta1?.AccountId == request.DestinationAccountId ? conta1 : conta2;

                    if (origem == null) return FailResponse("Conta origem não encontrada.", request.ReferenceId);
                    if (destino == null) return FailResponse("Conta destino não encontrada.", request.ReferenceId);

                    if (origem.Currency != request.Currency || destino.Currency != request.Currency)
                        return FailResponse("Moeda incompatível.", request.ReferenceId);

                    origem.Debit(request.Amount, request.ReferenceId);
                    destino.Credit(request.Amount, request.ReferenceId);

                    _contaRepository.Update(origem);
                    _contaRepository.Update(destino);

                    _transacaoProcessadaRepository.Adicionar(new TransacaoProcessada(request.ReferenceId, request.SourceAccountId, request.TransactionId, request.DestinationAccountId));

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    var transferenciaEvent = new TransferenciaRealizadaEvent
                    {
                        TransactionId = request.TransactionId,
                        SourceAccountId = request.SourceAccountId,
                        DestinationAccountId = request.DestinationAccountId,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = request.ReferenceId
                    };

                  //  await _publishEndpoint.Publish(transferenciaEvent, cancellationToken);

                    //await _messageProducer.PublishAsync(
                    //    transferenciaEvent,
                    //    exchangeName: "financeiro.transferencias.exchange", // Nome da sua Exchange
                    //    routingKey: $"transfer.success.{request.Currency.ToLower()}"); // Chave de roteamento


                    var txDebito = origem.Transacoes.First(t => t.ReferenceId == request.ReferenceId);


                    return SuccessResponse(origem, txDebito);
                });
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning(ex, "Falha de negócio na transferência.");
                return FailResponse(ex.Message, request.ReferenceId);
            }
            catch (ConflitoConcorrenciaException)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("Todas as tentativas de transferência falharam devido a conflito de concorrência: {ReferenceId}", request.ReferenceId);
                return FailResponse("Falha de concorrência após múltiplas tentativas. Tente novamente mais tarde.", request.ReferenceId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Erro crítico na transferência: {ReferenceId}", request.ReferenceId);
                return FailResponse("Erro interno.", request.ReferenceId);
            }
        }

        private static TransacaoResponse SuccessResponse(Conta conta, Transacao tx) => new(
             $"{tx.ReferenceId}-PROCESSED", "success", conta.SaldoTotal, conta.SaldoReservadoEmCentavos, conta.SaldoDisponivelEmCentavos, tx.Timestamp, null);

        private static TransacaoResponse FailResponse(string error, string refId) => new(
             $"{refId}-FAILED", "failed", 0, 0, 0, DateTime.UtcNow, error);
    }
}