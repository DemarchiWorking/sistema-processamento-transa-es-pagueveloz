using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    ///<summary>
    ///Evento publicado quando uma nova conta é criada.
    ///O CoreFinanceiro.API consumirá este evento para criar sua réplica local [ledger].
    ///</summary>
    ///<param name="AccountId">ID único da conta.</param>
    ///<param name="ClientId">ID do cliente proprietário.</param>
    ///<param name="LimiteDeCredito">Limite de crédito em centavos.</param>
    ///<param name="Status">Status inicial da conta.</param>
    ///<param name="Timestamp">Data/hora da criação.</param>
    public record ContaCreditadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,
        long ValorCreditado,
        string Currency,
        string? Metadata,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp);
}