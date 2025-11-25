using Microsoft.Extensions.DependencyInjection;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.CoreFinanceiro.Infra.Data; // Adicionar o using para CoreFinanceiroContext, se necessário
using PagueVeloz.CoreFinanceiro.Infra.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Servicos
{
    public class DomainEventService : IDomainEventService
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = false,
        };

        public DomainEventService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task PublishDomainEventsAsync()
        {
            var context = _serviceProvider.GetRequiredService<CoreFinanceiroContext>();

            var domainEntitiesWithEvents = context.ChangeTracker.Entries<Entity>()
                .Where(entry => entry.Entity.DomainEvents.Any())
                .Select(entry => entry.Entity)
                .ToList();

            if (!domainEntitiesWithEvents.Any())
            {
                return Task.CompletedTask;
            }

            var outboxMessages = new List<OutboxMessage>();

            foreach (var entity in domainEntitiesWithEvents)
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                {
                    var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), SerializerOptions);

                    var outboxMessage = new OutboxMessage(
                        domainEvent.GetType().FullName!,
                        payload
                    );
                    outboxMessages.Add(outboxMessage);
                }
            }
            context.DomainOutboxMessages.AddRange(outboxMessages);

            return Task.CompletedTask;
        }
    }
}