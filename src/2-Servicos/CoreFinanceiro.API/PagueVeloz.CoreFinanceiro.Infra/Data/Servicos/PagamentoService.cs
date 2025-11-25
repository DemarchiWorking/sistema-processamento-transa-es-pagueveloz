using Microsoft.Extensions.Logging;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs;
using PagueVeloz.CoreFinanceiro.Dominio.Enums;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.CoreFinanceiro.Infra.Persistencia;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Servicos
{
    public class PagamentoService : IPagamentoService
    {
        private readonly CoreFinanceiroContext _context;
        private readonly ILogger<PagamentoService> _logger;

        public PagamentoService(CoreFinanceiroContext context, ILogger<PagamentoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> ProcessPayment(Guid customerId, int amount)
        {
            _logger.LogInformation("Iniciando processamento de pagamento para ClienteId: {CustomerId} com valor {Amount}", customerId, amount);


            var newPaymentId = Guid.NewGuid();
            var payment = new Pagamento
            {
                Id = newPaymentId,
                CustomerId = customerId.ToString(),
                Amount = amount,
                Status = StatusPagamento.PEDDING,
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(payment);

            var paymentEvent = new PagamentoProcessadoEvent(newPaymentId, customerId, amount, "PROCESSED");


            var eventType = typeof(PagamentoProcessadoEvent).FullName!;
            var payload = JsonSerializer.Serialize(paymentEvent);


            var outboxMessage = new OutboxMessage(eventType, payload);

            _context.Set<OutboxMessage>().Add(outboxMessage);

            _logger.LogInformation("Mensagem Outbox criada para o evento {EventType}. Será salva atomicamente.", eventType);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Pagamento {PaymentId} processado e mensagem Outbox salva com sucesso.", newPaymentId);

            return newPaymentId;
        }
    }
}
