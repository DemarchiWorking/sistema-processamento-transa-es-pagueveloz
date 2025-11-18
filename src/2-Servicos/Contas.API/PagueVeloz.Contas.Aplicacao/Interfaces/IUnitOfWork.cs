using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Interfaces
{
    ///<summary>
    ///persistencia dos dados e a publicacao de eventos
    ///</summary>
    public interface IUnitOfWork
    {
        ///<summary>
        ///usado p salvar as mudanças (agregados | eeventos) no banco
        ///dentro de uma transacao.
        ///</summary>
        ///<returns>numero de linhas</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
