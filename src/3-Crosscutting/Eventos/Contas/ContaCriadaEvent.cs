using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    ///<summary>
    ///evento publicado quando uma nova conta e criada.
    ///sera consumidop esse evento para criar réplica local.
    ///</summary>
    public record ContaCriadaEvent(
        string AccountId,
        string ClientId,
        long LimiteDeCredito,
        StatusConta Status,
        DateTime Timestamp,
        long InitialBalance
    );
}