using PagueVeloz.Contas.Aplicacao.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Infra.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ContasDbContext _context;

        public UnitOfWork(ContasDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}