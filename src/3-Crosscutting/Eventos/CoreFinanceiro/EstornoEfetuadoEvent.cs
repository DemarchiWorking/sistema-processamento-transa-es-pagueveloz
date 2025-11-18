using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///Evento publicado quando um estorno [reversal] eh processado.
    ///</summary>
public record EstornoEfetuadoEvent(
    string AccountId,
    string TransactionId,
    string ReferenceId,
    string ReferenceIdTransacaoOriginal,  //id da transacao que esta sendo estornada
    long ValorEstornado,
    string Currency, 
    string? Metadata,
    SaldosContaDto NovosSaldos,
    DateTime Timestamp);
}