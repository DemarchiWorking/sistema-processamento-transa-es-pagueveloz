using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Servicos
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string eventType, string payload);

        Task Publish(string payload, string messageType);
    }
}