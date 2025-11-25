using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
public record EstornoEfetuadoEvent(
    string AccountId,
    string TransactionId,
    string ReferenceId,
    string ReferenceIdTransacaoOriginal, 
    long ValorEstornado,
    string Currency, 
    string? Metadata,
    SaldosContaDto NovosSaldos,
    DateTime Timestamp);
}