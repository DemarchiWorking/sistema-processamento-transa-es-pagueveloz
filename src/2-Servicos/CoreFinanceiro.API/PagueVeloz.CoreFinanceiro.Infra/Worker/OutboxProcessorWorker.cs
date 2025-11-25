using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.CoreFinanceiro.Infra.Data;
using PagueVeloz.CoreFinanceiro.Infra.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Worker
{
    namespace PagueVeloz.CoreFinanceiro.Infra.Workers
    {
        public class OutboxProcessorWorker : BackgroundService
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly ILogger<OutboxProcessorWorker> _logger;

            private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

            private const int MaxAttempts = 10;

            public OutboxProcessorWorker(
                IServiceProvider serviceProvider,
                ILogger<OutboxProcessorWorker> logger)
            {
                _serviceProvider = serviceProvider;
                _logger = logger;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("Outbox Processor Worker iniciado.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(PollingInterval, stoppingToken);

                    try
                    {
                        await ProcessOutboxMessagesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro fatal no Outbox Processor Worker. Tentando novamente na próxima iteração.");
                    }
                }

                _logger.LogInformation("Outbox Processor Worker finalizado.");
            }

            private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CoreFinanceiroContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessageBrokerPublisher>();

                var maxAttempts = MaxAttempts;
                var utcNow = DateTime.UtcNow;

                var messages = context.Set<OutboxMessage>()
                    .Where(m => m.ProcessedOnUtc == null &&                  
                                m.NextAttemptDateUtc <= utcNow &&            
                                m.Attempts < maxAttempts)                   
                    .OrderBy(m => m.CreatedOnUtc)
                    .ToList();

                if (!messages.Any()) return;

                _logger.LogInformation("Encontradas {Count} mensagens Outbox pendentes para processamento.", messages.Count);

                foreach (var message in messages)
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    try
                    {
                        await publisher.PublishAsync(message.EventType, message.Payload);

                        message.MarkAsProcessed();
                        _logger.LogInformation("Mensagem Outbox {MessageId} ({EventType}) publicada com sucesso.", message.Id, message.EventType);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = ex.ToString();
                        message.MarkAsFailed(errorMessage);
                        _logger.LogWarning("Falha ao publicar mensagem Outbox {MessageId}. Tentativa {Attempt}/{Max}: {Error}",
                            message.Id, message.Attempts, MaxAttempts, errorMessage);
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}