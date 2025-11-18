using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Dominio.Enums
{
    ///<summary>
    ///define os estados de ciclo de vida de uma conta no contexto de gestao [AccountManagement].
    ///</summary>
    public enum StatusConta
    {
        /// <summary>
        /// Conta apta a transacionar.
        /// </summary>
        Active,

        /// <summary>
        /// Conta desativada, não transaciona.
        /// </summary>
        Inactive,

        /// <summary>
        /// Conta bloqueada por regra de negócio (ex: fraude, adm.), não transaciona.
        /// </summary>
        Blocked
    }
}