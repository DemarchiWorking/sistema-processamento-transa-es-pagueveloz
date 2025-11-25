using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    public record StatusDaContaAlteradoEvent(
        string AccountId,
        StatusConta NovoStatus,
        DateTime Timestamp);
}