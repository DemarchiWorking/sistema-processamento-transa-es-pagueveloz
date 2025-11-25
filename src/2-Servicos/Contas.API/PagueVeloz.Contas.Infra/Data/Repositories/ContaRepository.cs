using Microsoft.EntityFrameworkCore;
using PagueVeloz.Contas.Aplicacao.Interfaces;
using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Infra.Data.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly ContasDbContext _context;

        public ContaRepository(ContasDbContext context)
        {
            _context = context;
        }

        public async Task<Conta> GetByIdAsync(string contaId, CancellationToken cancellationToken)
        {
            return await _context.Contas
                //.Include(c => c.Transacoes)
                .FirstOrDefaultAsync(c => c.Id == contaId, cancellationToken);
        }

        public void Add(Conta conta)
        {
            _context.Contas.Add(conta);
        }

        public void Update(Conta conta)
        {
            _context.Contas.Update(conta);
        }
        public async Task<long> ObterProximoNumeroContaAsync()
        {
            var sql = "SELECT nextval('\"Contas\".\"ContaSeq\"')";

            var result = await _context.Database
                .SqlQueryRaw<long>(sql)
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}