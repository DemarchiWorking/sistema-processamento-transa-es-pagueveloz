using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Interfaces
{
    public interface ITransacaoProcessadaRepository
    {
        Task<bool> JaProcessadaAsync(string referenceId, CancellationToken cancellationToken);

        void Adicionar(TransacaoProcessada transacao);
    }
}