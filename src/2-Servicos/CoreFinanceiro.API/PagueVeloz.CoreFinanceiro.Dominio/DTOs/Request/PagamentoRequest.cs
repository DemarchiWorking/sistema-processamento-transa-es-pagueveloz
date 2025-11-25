using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.DTOs.Request
{
    public class PagamentoRequest
    {
        [Required(ErrorMessage = "O CustomerId é obrigatório.")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "O valor (Amount) é obrigatório.")]
        public int Amount { get; set; }
    }
}