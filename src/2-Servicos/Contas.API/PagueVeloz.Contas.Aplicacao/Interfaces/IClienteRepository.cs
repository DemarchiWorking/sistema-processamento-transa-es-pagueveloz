using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Interfaces
{
    ///<summary>
    ///abstracao do repositorio de clientes.
    ///</summary>
    public interface IClienteRepository
    {
        ///<summary>
        ///verifica se um cliente existe pelo seu ID.
        ///</summary>
        Task<bool> ExisteAsync(string clienteId, CancellationToken cancellationToken);


    }
}
