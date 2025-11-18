using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///Evento publicado quando um valor eh movido de disponivel p/ Reservado.
    ///</summary>
    public record ReservaEfetuadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,
        long ValorReservado,
        string Currency,
        string? Metadata,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp);
}