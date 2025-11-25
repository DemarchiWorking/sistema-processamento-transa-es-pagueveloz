using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas.Interface
{
    public interface IContaAlteradaEvent 
    {
        public string AccountId { get; }
        public string TipoOperacao { get; }
        public long Valor { get; }
        public string ReferenceId { get; }
        public long SaldoDisponivelEmCentavos { get; }
        public long SaldoReservadoEmCentavos { get; }
        public DateTime Timestamp { get; }
    }
}