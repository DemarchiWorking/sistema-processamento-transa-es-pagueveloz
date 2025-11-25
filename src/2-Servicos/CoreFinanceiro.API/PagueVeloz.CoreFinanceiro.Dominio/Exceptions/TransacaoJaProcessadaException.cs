using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Exceptions
{
    public class TransacaoJaProcessadaException : DomainException
    {
        public TransacaoJaProcessadaException(string referenceId)
            : base($"A operação com o ID de referência '{referenceId}' já foi processada. Idempotência garantida.")
        {
        }
        protected TransacaoJaProcessadaException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}