using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly CoreFinanceiroContext _context;

        public IdempotenciaRepository(CoreFinanceiroContext context) => _context = context;

        public async Task<TransacaoProcessada?> GetByReferenceIdAsync(string referenceId)
        {
            return await _context.TransacoesProcessadas.FindAsync(referenceId);
        }

        public void Add(TransacaoProcessada operation)
        {
            _context.TransacoesProcessadas.Add(operation);
        }
    }
}