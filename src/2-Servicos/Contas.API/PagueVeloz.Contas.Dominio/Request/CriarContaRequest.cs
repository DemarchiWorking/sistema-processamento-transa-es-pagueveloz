using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Dominio.Request
{
    public record CriarContaRequest(
        string ClienteId,
        long InitialBalance,
        long CreditLimit
    );
}