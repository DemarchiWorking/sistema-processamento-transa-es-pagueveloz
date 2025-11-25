using PagueVeloz.CoreFinanceiro.Dominio.DTOs;
using PagueVeloz.CoreFinanceiro.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.DTOs
{
    public class Pagamento
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public int Amount { get; set; }
        public StatusPagamento Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}