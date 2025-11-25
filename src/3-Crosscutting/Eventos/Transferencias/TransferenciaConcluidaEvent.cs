using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Eventos.Transferencias
{
    public record TransferenciaConcluidaEvent
    {
        public Guid TransferenciaId { get; init; } 
        public string Status { get; init; } 
        public string MotivoFalha { get; init; }
        public DateTime Timestamp { get; init; }
        public string ContaOrigemId { get; init; }
        public string ContaDestinoId { get; init; }
        public long Valor { get; init; }
    }
}