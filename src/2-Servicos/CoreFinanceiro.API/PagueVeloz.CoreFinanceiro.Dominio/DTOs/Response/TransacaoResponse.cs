using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.DTOs.Response
{
    public record TransacaoResponse(
        string TransactionId,
        string Status,
        long Balance,
        long ReservedBalance,
        long AvailableBalance,
        DateTime Timestamp,
        string? ErrorMessage
    );
}