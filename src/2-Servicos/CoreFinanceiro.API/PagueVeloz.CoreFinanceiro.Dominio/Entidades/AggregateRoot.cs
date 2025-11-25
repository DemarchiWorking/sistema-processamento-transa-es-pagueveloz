using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{
    public abstract class AggregateRoot : IAggregateRoot 
    {
        public Guid Id { get; protected set; }

        private readonly List<object> _domainEvents = new();

        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly(); 

        protected void AddDomainEvent(object eventItem) => _domainEvents.Add(eventItem); 

        public void ClearDomainEvents() => _domainEvents.Clear(); 
    }
}