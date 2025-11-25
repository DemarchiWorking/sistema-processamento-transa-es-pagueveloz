using Microsoft.EntityFrameworkCore;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Repositories
{
    public class TransacaoProcessadaRepository : ITransacaoProcessadaRepository
    {
        private readonly CoreFinanceiroContext _context;

        public TransacaoProcessadaRepository(CoreFinanceiroContext context)
        {
            _context = context;
        }

        public void Adicionar(TransacaoProcessada transacao)
        {
            _context.TransacoesProcessadas.Add(transacao);
        }

        public async Task<bool> JaProcessadaAsync(string referenceId, CancellationToken cancellationToken)
        {
            return await _context.TransacoesProcessadas
                .AsNoTracking()
                .AnyAsync(t => t.ReferenceId == referenceId, cancellationToken);
        }
    }
}
