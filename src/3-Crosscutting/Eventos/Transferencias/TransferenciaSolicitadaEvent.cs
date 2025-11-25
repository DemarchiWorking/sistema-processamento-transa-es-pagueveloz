using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Transferencias
{
    public record TransferenciaSolicitadaEvent(
        Guid TransferenciaId,    
        string ContaOrigemId,
        string ContaDestinoId,
        long Valor,                
        string Currency,
        string ReferenceId,        
        string? Metadata);
}