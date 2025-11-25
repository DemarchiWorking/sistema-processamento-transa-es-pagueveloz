using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.CoreFinanceiro
{
    public record SaldosContaDto(
        long Balance,
        long ReservedBalance,
        long AvailableBalance);
}