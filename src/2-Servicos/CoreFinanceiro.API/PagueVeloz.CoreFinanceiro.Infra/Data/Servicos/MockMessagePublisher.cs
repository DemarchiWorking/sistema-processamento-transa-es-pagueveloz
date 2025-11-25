using Microsoft.Extensions.Logging;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Servicos
{
    public class MockMessagePublisher : IMessagePublisher, IMessageBrokerPublisher
    {
        private readonly ILogger<MockMessagePublisher> _logger;

        public MockMessagePublisher(ILogger<MockMessagePublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync(string eventType, string payload)
        {

            _logger.LogInformation("SIMULAÇÃO DE ENVIO: Evento '{EventType}' publicado com sucesso! Payload de {Length} bytes.",
                eventType, payload.Length);

            return Task.CompletedTask;
        }
        public Task Publish(string payload, string messageType)
        {
            _logger.LogInformation(
                "MOCK PUBLISHER: Mensagem Outbox simulada enviada. Tipo: {MessageType}",
                messageType
            );

            return Task.Delay(50);
        }
    }
}
