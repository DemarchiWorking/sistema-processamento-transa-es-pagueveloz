using PagueVeloz.Eventos.Contas.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Contas
{
    public class ContaAlteradaEvent : IContaAlteradaEvent
    {
        public string AccountId { get; init; }
        public string TipoOperacao { get; init; }
        public long Valor { get; init; }
        public string ReferenceId { get; init; }
        public long SaldoDisponivelEmCentavos { get; init; }
        public long SaldoReservadoEmCentavos { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        public ContaAlteradaEvent(string accountId, string tipoOperacao, long valor, string referenceId, long saldoDisponivel, long saldoReservado)
        {
            AccountId = accountId;
            TipoOperacao = tipoOperacao;
            Valor = valor;
            ReferenceId = referenceId;
            SaldoDisponivelEmCentavos = saldoDisponivel;
            SaldoReservadoEmCentavos = saldoReservado;
        }
    }
}
/*
    public class ContaAlteradaEvent
    {
        private string accountId;
        private string tipo;
        private long valor;
        private string referenceId;
        private long saldoDisponivelEmCentavos;
        private long saldoReservadoEmCentavos;

        public ContaAlteradaEvent(string accountId, string tipo, long valor, string referenceId, long saldoDisponivelEmCentavos, long saldoReservadoEmCentavos)
        {
            this.accountId = accountId;
            this.tipo = tipo;
            this.valor = valor;
            this.referenceId = referenceId;
            this.saldoDisponivelEmCentavos = saldoDisponivelEmCentavos;
            this.saldoReservadoEmCentavos = saldoReservadoEmCentavos;
        }
    }
}*/