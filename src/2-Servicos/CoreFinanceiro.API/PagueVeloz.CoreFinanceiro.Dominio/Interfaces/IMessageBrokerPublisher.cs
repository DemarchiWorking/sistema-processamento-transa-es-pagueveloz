using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Interfaces
{
    public interface IMessageBrokerPublisher
    {
        Task PublishAsync(string topic, string message);
    }
}