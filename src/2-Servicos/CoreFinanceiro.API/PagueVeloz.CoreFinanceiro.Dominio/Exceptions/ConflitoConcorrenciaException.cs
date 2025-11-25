using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Exceptions
{
    public class ConflitoConcorrenciaException : Exception
    {
        public ConflitoConcorrenciaException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}