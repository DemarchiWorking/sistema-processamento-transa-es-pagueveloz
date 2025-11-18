using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///Evento publicado quando um valor Reservado eh capturado [confirmado].
    ///</summary>
    public record ReservaCapturadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,
        long ValorCapturado,
        string Currency,
        string? Metadata,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp);
}