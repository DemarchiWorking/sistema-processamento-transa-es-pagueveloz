using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    ///<summary>
    ///Evento publicado quando o limite de crédito de uma conta e alterado.
    ///O CoreFinanceiro.API consumira este evento para atualizar sua copia local do limite.
    ///</summary>
    ///<param name="AccountId">ID unico da conta.</param>
    ///<param name="NovoLimiteDeCredito">O novo limite de credito em centavos.</param>
    ///<param name="Timestamp">Data/hora da alteracao.</param>
    public record LimiteDeCreditoAlteradoEvent(
        string AccountId,
        long NovoLimiteDeCredito,
        DateTime Timestamp);
}