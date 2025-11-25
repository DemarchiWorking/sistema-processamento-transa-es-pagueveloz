using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Interfaces
{
    public interface IClienteRepository
    {
        Task<bool> ExisteAsync(string clienteId, CancellationToken cancellationToken);

        void Adicionar(Dominio.Aggregates.Cliente cliente); 
    }
}