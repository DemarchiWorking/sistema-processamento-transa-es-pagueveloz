using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class ProcessarEstornoCommandHandler : IRequestHandler<ProcessarEstornoCommand, TransacaoResponse>
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoProcessadaRepository _transacaoProcessadaRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessarEstornoCommandHandler> _logger;

        public ProcessarEstornoCommandHandler(
            IContaRepository contaRepository,
            ITransacaoProcessadaRepository transacaoProcessadaRepository,
            IPublishEndpoint publishEndpoint,
            IUnitOfWork unitOfWork,
            ILogger<ProcessarEstornoCommandHandler> logger)
        {
            _contaRepository = contaRepository;
            _transacaoProcessadaRepository = transacaoProcessadaRepository;
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TransacaoResponse> Handle(ProcessarEstornoCommand request, CancellationToken cancellationToken)
        {
            if (await _transacaoProcessadaRepository.JaProcessadaAsync(request.ReferenceId, cancellationToken))
            {
                _logger.LogWarning("Transação {ReferenceId} já processada (idempotência).", request.ReferenceId);
                return new TransacaoResponse($"DUPLICATE-{request.ReferenceId}", "success", 0, 0, 0, DateTime.UtcNow, null);
            }

            string? originalReferenceId = null;

            if (request.Metadata != null && request.Metadata.TryGetValue("originalReferenceId", out object? objValue))
            {
                if (objValue is JsonElement jsonElement)
                {
                    originalReferenceId = jsonElement.ValueKind == JsonValueKind.String
                        ? jsonElement.GetString()
                        : jsonElement.ToString();
                }
                else if (objValue != null)
                {
                    originalReferenceId = objValue.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(originalReferenceId))
            {
                _logger.LogWarning("Estorno {ReferenceId} falhou: 'originalReferenceId' ausente ou vazio no metadata.", request.ReferenceId);
                return FailResponse("Metadata 'originalReferenceId' é obrigatório para estorno.", request.ReferenceId);
            }

            var conta = await _contaRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (conta == null)
            {
                return FailResponse("Conta não encontrada.", request.ReferenceId);
            }

            if (request.Currency != conta.Currency)
            {
                return FailResponse($"Moeda inválida. Esperado: {conta.Currency}", request.ReferenceId, conta);
            }

            try
            {
                conta.Estornar(request.ReferenceId, originalReferenceId);//request.Amount,

                _contaRepository.Update(conta);

                _transacaoProcessadaRepository.Adicionar(new TransacaoProcessada(request.ReferenceId, request.AccountId, request.TransactionId, null));

                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Conflito de concorrência no Estorno: {ReferenceId}", request.ReferenceId);
                    throw new ConflitoConcorrenciaException("Conflito de concorrência detectado durante o estorno.", ex);
                }

                var transacaoRealizada = conta.Transacoes.First(t => t.ReferenceId == request.ReferenceId);

                string metadataJson = request.Metadata != null ? JsonSerializer.Serialize(request.Metadata) : "{}";

                var evento = new EstornoEfetuadoEvent(
                    conta.AccountId,
                    transacaoRealizada.Id.ToString(),
                    request.ReferenceId,
                    originalReferenceId,
                    request.Amount,
                    request.Currency,
                    metadataJson,
                    new SaldosContaDto(
                        conta.SaldoTotal,
                        conta.SaldoReservadoEmCentavos,
                        conta.SaldoDisponivelEmCentavos
                    ),
                    transacaoRealizada.Timestamp
                );

                await _publishEndpoint.Publish(evento, cancellationToken);

                conta.ClearDomainEvents();

                return SuccessResponse(conta, transacaoRealizada);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Falha na regra de negócio (Estorno): {ReferenceId}", request.ReferenceId);
                return FailResponse(ex.Message, request.ReferenceId, conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico ao processar estorno: {ReferenceId}", request.ReferenceId);
                return FailResponse("Erro interno do servidor.", request.ReferenceId, conta);
            }
        }
        private static TransacaoResponse SuccessResponse(Conta conta, Transacao transacao) => new(
            TransactionId: $"{transacao.ReferenceId}-PROCESSED",
            Status: "success",
            Balance: conta.SaldoTotal,
            ReservedBalance: conta.SaldoReservadoEmCentavos,
            AvailableBalance: conta.SaldoDisponivelEmCentavos,
            Timestamp: transacao.Timestamp,
            ErrorMessage: null
        );

        private static TransacaoResponse FailResponse(string error, string referenceId, Conta? conta = null) => new(
            TransactionId: $"{referenceId}-FAILED",
            Status: "failed",
            Balance: conta?.SaldoTotal ?? 0,
            ReservedBalance: conta?.SaldoReservadoEmCentavos ?? 0,
            AvailableBalance: conta?.SaldoDisponivelEmCentavos ?? 0,
            Timestamp: DateTime.UtcNow,
            ErrorMessage: error
        );
    }
}
