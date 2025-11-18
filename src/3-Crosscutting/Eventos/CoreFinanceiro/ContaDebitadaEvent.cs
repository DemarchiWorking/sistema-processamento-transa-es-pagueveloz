using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///Evento publicado quando um debito e processado com sucesso.
    ///Consumido pelo Transferencias.API [Saga].
    ///</summary>
    ///<param name="AccountId">ID unico da conta.</param>
    ///<param name="TransactionId">ID da transacao processada.</param>
    ///<param name="ReferenceId">ID de idempotencia original.</param>
    ///<param name="ValorDebitado">Valor debitado em centavos.</param>
    ///<param name="NovosSaldos">O estado atualizado dos saldos apos o debito.</param>
    ///<param name="Timestamp">Data/hora da transacao.</param>
    public record ContaDebitadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,
        long ValorDebitado,
        string Currency,
        string? Metadata,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp);
}