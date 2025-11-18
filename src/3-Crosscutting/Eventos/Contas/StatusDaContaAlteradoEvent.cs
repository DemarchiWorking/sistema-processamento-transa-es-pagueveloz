using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    ///<summary>
    ///Evento publicado quando o status de uma conta e alterado.
    ///O CoreFinanceiro.API consumira este evento para atualizar sua copia local do status.
    ///</summary>
    ///<param name="AccountId">ID unico da conta.</param>
    ///<param name="NovoStatus">O novo status da conta.</param>
    ///<param name="Timestamp">Data/hora da alteração.</param>
    public record StatusDaContaAlteradoEvent(
        string AccountId,
        StatusConta NovoStatus,
        DateTime Timestamp);
}