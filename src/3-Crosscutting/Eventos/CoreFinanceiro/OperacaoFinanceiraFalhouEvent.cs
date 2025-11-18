using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///evento publicado quando qualquer operacao de saldo falha.
    ///Consumido pelo Transferencias.API [Saga] para iniciar um rollback.
    ///</summary>
    ///<param name="AccountId">id unico da conta.</param>
    ///<param name="ReferenceId">idde idempotencia original.</param>
    ///<param name="MensagemErro">motivo da falha.</param>
    ///<param name="Operacao">tipo da operacao que falhou.</param>
    ///<param name="Timestamp">Data/hora da falha.</param>
    public record OperacaoFinanceiraFalhouEvent(
        string AccountId,
        string ReferenceId,
        string MensagemErro,
        string Operacao,
        long Valor, //adicionado para contexto da falha
        string Currency,
        string? Metadata, 
        DateTime Timestamp);
}