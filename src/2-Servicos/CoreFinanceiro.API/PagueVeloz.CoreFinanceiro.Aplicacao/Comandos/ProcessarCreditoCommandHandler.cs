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
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Comandos
{
    public class ProcessarCreditoCommandHandler : IRequestHandler<ProcessarCreditoCommand, TransacaoResponse>
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacaoProcessadaRepository _transacaoProcessadaRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessarCreditoCommandHandler> _logger;

        public ProcessarCreditoCommandHandler(
            IContaRepository contaRepository,
            ITransacaoProcessadaRepository transacaoProcessadaRepository,
            IPublishEndpoint publishEndpoint,
            IUnitOfWork unitOfWork, 
            ILogger<ProcessarCreditoCommandHandler> logger)
        {
            _contaRepository = contaRepository;
            _transacaoProcessadaRepository = transacaoProcessadaRepository;
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork; 
            _logger = logger;
        }

        public async Task<TransacaoResponse> Handle(ProcessarCreditoCommand request, CancellationToken cancellationToken)
        {
            if (await _transacaoProcessadaRepository.JaProcessadaAsync(request.ReferenceId, cancellationToken))
            {
                _logger.LogWarning("Transação {ReferenceId} já processada (idempotência).", request.ReferenceId);
                return new TransacaoResponse($"DUPLICATE-{request.ReferenceId}", "success", 0, 0, 0, DateTime.UtcNow, null);
            }

            var conta = await _contaRepository.GetByIdAsync(request.AccountId, cancellationToken);
            if (conta == null) return FailResponse("Conta não encontrada.", request.ReferenceId);

            if (request.Currency != conta.Currency)
                return FailResponse($"Moeda inválida. Esperado: {conta.Currency}", request.ReferenceId, conta);

            try
            {
                conta.Credit(request.Amount, request.ReferenceId);

                _contaRepository.Update(conta);

                _transacaoProcessadaRepository.Adicionar(new TransacaoProcessada(request.ReferenceId, request.AccountId, request.ReferenceId, null));

                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Conflito de concorrência no crédito: {ReferenceId}", request.ReferenceId);
                    throw new ConflitoConcorrenciaException("Conflito de concorrência detectado durante o crédito.", ex);
                }

                var transacao = conta.Transacoes.First(t => t.ReferenceId == request.ReferenceId);

                string metadataJson = request.Metadata != null ? JsonSerializer.Serialize(request.Metadata) : "{}";

                var evento = new ContaCreditadaEvent(
                    conta.AccountId,
                    transacao.Id.ToString(),
                    request.ReferenceId,
                    request.Amount,
                    request.Currency,
                    metadataJson,
                    new SaldosContaDto(conta.SaldoTotal, conta.SaldoReservadoEmCentavos, conta.SaldoDisponivelEmCentavos),
                    transacao.Timestamp
                );

                await _publishEndpoint.Publish(evento, cancellationToken);
                conta.ClearDomainEvents();

                return SuccessResponse(conta, transacao);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Falha de negócio no crédito: {ReferenceId}", request.ReferenceId);
                return FailResponse(ex.Message, request.ReferenceId, conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno no crédito: {ReferenceId}", request.ReferenceId);
                return FailResponse("Erro interno.", request.ReferenceId, conta);
            }
        }

        private static TransacaoResponse SuccessResponse(Conta conta, Transacao tx) => new(
            $"{tx.ReferenceId}-PROCESSED", "success", conta.SaldoTotal, conta.SaldoReservadoEmCentavos, conta.SaldoDisponivelEmCentavos, tx.Timestamp, null);

        private static TransacaoResponse FailResponse(string error, string refId, Conta? c = null) => new(
            $"{refId}-FAILED", "failed", c?.SaldoTotal ?? 0, c?.SaldoReservadoEmCentavos ?? 0, c?.SaldoDisponivelEmCentavos ?? 0, DateTime.UtcNow, error);
    }
}