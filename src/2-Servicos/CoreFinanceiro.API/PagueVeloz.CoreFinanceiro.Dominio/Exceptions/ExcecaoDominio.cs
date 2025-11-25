using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Exceptions
{
    public class ExcecaoDominio : Exception
    {
        public ExcecaoDominio(string message) : base(message) { }
    }

    public class SaldoInsuficienteExcecao : ExcecaoDominio
    {
        public SaldoInsuficienteExcecao(string message) : base(message) { }
    }
}