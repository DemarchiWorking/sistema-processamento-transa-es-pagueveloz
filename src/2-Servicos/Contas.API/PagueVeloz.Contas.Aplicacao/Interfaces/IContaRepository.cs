using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Interfaces
{
    public interface IContaRepository
    {
        Task<Conta> GetByIdAsync(string contaId, CancellationToken cancellationToken);

        void Add(Conta conta);

        void Update(Conta conta);
        Task<long> ObterProximoNumeroContaAsync();

    }
}