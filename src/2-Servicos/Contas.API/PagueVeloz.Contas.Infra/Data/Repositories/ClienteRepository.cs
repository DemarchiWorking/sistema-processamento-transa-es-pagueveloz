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
    public class ClienteRepository : IClienteRepository
    {
        private readonly ContasDbContext _context;

        public ClienteRepository(ContasDbContext context)
        {
            _context = context;
        }

        public void Adicionar(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
        }

        public async Task<bool> ExisteAsync(string clienteId, CancellationToken cancellationToken)
        {
            return await _context.Clientes
        .AsNoTracking()
        .AnyAsync(c => c.Id == clienteId, cancellationToken);
        
        //    return await _context.Clientes
        //        .AsNoTracking()
        //        .AnyAsync(c => c.Id == clienteId, cancellationToken);
        }
    }
}