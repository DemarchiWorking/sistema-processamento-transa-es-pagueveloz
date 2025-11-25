using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Transferencias.Aplicacao.Comandos
{
    public record TransferCommand(
            string AccountId,
            string DestinationAccountId,
            long Amount,
            string Currency,
            string ReferenceId,
            object? Metadata);
}
