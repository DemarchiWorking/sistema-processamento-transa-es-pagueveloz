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

        ///<summary>
        ///salva as alteracoes. 
        ///alert:configurar masstransit outbox,
        ///para salva os agregados e eventos na mesma transacao de banco;
        ///</summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}