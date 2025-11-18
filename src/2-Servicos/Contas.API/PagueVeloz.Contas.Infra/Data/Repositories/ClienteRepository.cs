using Microsoft.EntityFrameworkCore;
using PagueVeloz.Contas.Aplicacao.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Infra.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ContasDbContext _context;

        public ClienteRepository(ContasDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(string clienteId, CancellationToken cancellationToken)
        {
            //checar existencia.
            return await _context.Clientes
                .AsNoTracking() //queries de leitura
                .AnyAsync(c => c.Id == clienteId, cancellationToken);
        }
    }
}