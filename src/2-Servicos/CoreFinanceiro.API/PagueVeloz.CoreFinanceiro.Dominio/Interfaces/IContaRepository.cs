using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;

namespace PagueVeloz.CoreFinanceiro.Dominio.Interfaces
{
    public interface IContaRepository
    {
        Task<Conta> GetByIdAsync(string accountId, CancellationToken cancellationToken);
        Task<Conta> GetByAccountNumber(string numeroConta, CancellationToken cancellationToken);
        Task<Movimento> GetMovimentoByIdAsync(string movimentoId, CancellationToken cancellationToken);
        void Add(Conta conta);
        void Update(Conta conta);
        Task<TransacaoProcessada> GetAsynchronousProcessedTransaction(string referenceId);
        void RegisterTransactionProcessed(TransacaoProcessada transacao);
    }
}
 