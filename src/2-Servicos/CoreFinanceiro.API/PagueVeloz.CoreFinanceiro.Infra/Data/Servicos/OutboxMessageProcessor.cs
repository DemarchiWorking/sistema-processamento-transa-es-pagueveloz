using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using PagueVeloz.CoreFinanceiro.Infra.Data; 
using PagueVeloz.CoreFinanceiro.Infra.Persistencia; 
using MTOutboxStateArquivo = PagueVeloz.CoreFinanceiro.Infra.Persistencia.OutboxMessage;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Servicos
{

    public class OutboxMessageProcessor : BackgroundService
    {
        private const int BatchSize = 20;
        private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
        private const int MaxAttempts = 8;

        private readonly ILogger<OutboxMessageProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessagePublisher _publisher;

        public OutboxMessageProcessor(
            ILogger<OutboxMessageProcessor> logger,
            IServiceScopeFactory scopeFactory,
            IMessagePublisher publisher)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxMessageProcessor iniciado e pronto para processar mensagens.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessages(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro crítico no ciclo principal do OutboxMessageProcessor.");
                }

                await Task.Delay(PollingInterval, stoppingToken);
            }

            _logger.LogInformation("OutboxMessageProcessor finalizado.");
        }

        private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreFinanceiroContext>();
            var messagesToProcess = await context.Set<MTOutboxStateArquivo>()
                .Where(m => m.ProcessedOnUtc == null && m.NextAttemptDateUtc <= DateTime.UtcNow)
                .OrderBy(m => m.CreatedOnUtc)
                .Take(BatchSize)
                .ToListAsync(stoppingToken);

            if (!messagesToProcess.Any())
            {
                _logger.LogTrace("Nenhuma mensagem Outbox pendente para processamento.");
                return;
            }

            _logger.LogInformation("Encontradas {Count} mensagens Outbox pendentes para envio.", messagesToProcess.Count);

            foreach (var message in messagesToProcess)
            {
                if (stoppingToken.IsCancellationRequested) return;

                await ProcessMessage(context, message);
            }
        }

        private async Task ProcessMessage(CoreFinanceiroContext context, MTOutboxStateArquivo message)
        {
            try
            {
                if (message.Attempts >= MaxAttempts)
                {
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = $"Falha permanente após {MaxAttempts} tentativas. Movido para DLQ virtual.";
                    _logger.LogWarning("Mensagem {Id} excedeu o limite de {MaxAttempts} tentativas. Considerada falha permanente.", message.Id, MaxAttempts);
                    return; 
                }

                _logger.LogInformation("Tentativa {Attempt}: Enviando evento Id {Id}, Tipo={Type}",
                     message.Attempts + 1, message.Id, message.Type);

                await _publisher.Publish(message.Payload, message.Type);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.NextAttemptDateUtc = null;
                message.Error = null;

                _logger.LogInformation("Mensagem Outbox (ID: {Id}, Tipo: {Type}) enviada com sucesso.", message.Id, message.Type);
            }
            catch (Exception ex)
            {
                message.Attempts++;

                double delaySeconds = Math.Pow(2, Math.Min(message.Attempts, 6));
                message.NextAttemptDateUtc = DateTime.UtcNow.AddSeconds(delaySeconds);

                message.Error = ex.Message.Substring(0, Math.Min(ex.Message.Length, 2048));

                _logger.LogWarning(
                    "Falha ao enviar mensagem Outbox (ID: {Id}, Tentativa: {Attempts}). Próxima tentativa agendada para {NextAttemptDate:T}. Erro: {Error}",
                    message.Id, message.Attempts, message.NextAttemptDateUtc, ex.Message);
            }
            finally
            {
                await context.SaveChangesAsync();
            }
        }
    }
}