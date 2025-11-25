using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Repositories
{
    public class ProjetorRepository<T> : IProjetorRepository<T> where T : class
    {
        public Task AddAsync(T entity)
        {
            return Task.CompletedTask;
        }
    }
}