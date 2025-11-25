using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    public class PagamentoProcessadoEvent
    {
        public Guid PaymentId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ProcessedAtUtc { get; set; }
        public string Status { get; set; }

        public PagamentoProcessadoEvent(Guid paymentId, Guid customerId, decimal amount, string status)
        {
            PaymentId = paymentId;
            CustomerId = customerId;
            Amount = amount;
            ProcessedAtUtc = DateTime.UtcNow;
            Status = status;
        }
    }
}
