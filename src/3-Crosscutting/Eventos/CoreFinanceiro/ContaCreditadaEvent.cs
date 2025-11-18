using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///Evento publicado quando um credito eh processado com sucesso.
    ///Consumido pelo Transferencias.API e por outros sistemas.
    ///</summary>
    ///<param name="AccountId">ID unico da conta.</param>
    ///<param name="TransactionId">ID da transacao processada.</param>
    ///<param name="ReferenceId">ID de idempotencia original.</param>
    ///<param name="ValorCreditado">Valor creditado em centavos.</param>
    ///<param name="NovosSaldos">O estado atualizado dos saldos apos o credito.</param>
    ///<param name="Timestamp">Data/hora da transacao.</param>
    public record ContaCreditadaEvent(
        string AccountId,
        string TransactionId,
        string ReferenceId,
        long ValorCreditado,
        SaldosContaDto NovosSaldos,
        DateTime Timestamp);
}