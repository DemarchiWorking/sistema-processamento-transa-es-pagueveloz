using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{

    public interface IDomainEvent { }


    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        protected void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }


        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;
            if (Id == Guid.Empty || other.Id == Guid.Empty)
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity? a, Entity? b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity? a, Entity? b) => !(a == b);

        public override int GetHashCode() => Id.GetHashCode();
    }
}