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

        public void Adicionar(Conta conta)
        {
            //adiciona ao changetracker [ef Core].
            //unit of Work | savechanges;
            _context.Contas.Add(conta);
        }
    }
}