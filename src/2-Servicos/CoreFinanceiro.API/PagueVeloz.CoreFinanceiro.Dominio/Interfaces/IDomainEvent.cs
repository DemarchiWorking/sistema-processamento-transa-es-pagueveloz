using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Interfaces
{

    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}