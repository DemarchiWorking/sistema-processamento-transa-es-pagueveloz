using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    ///<summary>
    ///dto compartilhado para representar o estado dos saldos de uma conta
    ///apos uma transacao.
    ///</summary>
    ///<param name="Balance"> saldo total [disponivel+reservado] em centavos.</param>
    ///<param name="ReservedBalance">saldo reservado em centavos.</param>
    ///<param name="AvailableBalance">saldo disponivel [balance/Reserved] em centavos.</param>
    public record SaldosContaDto(
        long Balance,
        long ReservedBalance,
        long AvailableBalance);
}