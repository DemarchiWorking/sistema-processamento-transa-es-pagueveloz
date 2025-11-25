using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Interfaces
{

    public interface IIdempotenciaRepository
    {
        Task<TransacaoProcessada?> GetByReferenceIdAsync(string referenceId);
        void Add(TransacaoProcessada operation);
    }
}

