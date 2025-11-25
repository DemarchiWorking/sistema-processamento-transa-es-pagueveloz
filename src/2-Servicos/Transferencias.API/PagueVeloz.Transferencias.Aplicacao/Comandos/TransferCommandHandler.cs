using MassTransit;
using PagueVeloz.Eventos.Transferencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Transferencias.Aplicacao.Comandos
{
    public class TransferCommandHandler
    {
        private readonly IBus _bus;

        public TransferCommandHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task<Guid> Handle(TransferCommand command)
        {
            var sagaId = Guid.NewGuid();

            await _bus.Publish(new TransferenciaSolicitadaEvent(
                TransferenciaId: sagaId,
                ContaOrigemId: command.AccountId,
                ContaDestinoId: command.DestinationAccountId,
                Valor: command.Amount,
                Currency: command.Currency,
                ReferenceId: command.ReferenceId,
                Metadata: command.Metadata?.ToString() 
            ));

            return sagaId;
        }
    }
}