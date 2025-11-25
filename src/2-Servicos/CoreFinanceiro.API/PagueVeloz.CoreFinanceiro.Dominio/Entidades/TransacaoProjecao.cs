using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{
    public class TransacaoProjecao
    {
        public Guid Id { get; set; }
        public string AccountId { get; set; }
        public string ReferenceId { get; set; } 
        public string OriginalReferenceId { get; set; }
        public TipoTransacao Tipo { get; set; }
        public long ValorEmCentavos { get; set; }
        public string Currency { get; set; }
        public long SaldoAposTransacao { get; set; }
        public DateTime Timestamp { get; set; }
    }
}