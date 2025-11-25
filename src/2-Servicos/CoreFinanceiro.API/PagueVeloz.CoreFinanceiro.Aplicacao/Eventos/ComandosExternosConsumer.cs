using MassTransit;
using MediatR;
using PagueVeloz.CoreFinanceiro.Aplicacao.Comandos;
using PagueVeloz.Eventos.Comandos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Eventos
{
    public class ComandosExternosConsumer :
        IConsumer<RealizarDebitoCommand>,
        IConsumer<RealizarCreditoCommand>,
        IConsumer<RealizarEstornoCommand>
    {
        private readonly IMediator _mediator;

        public ComandosExternosConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<RealizarDebitoCommand> context)
        {
            var msg = context.Message;


            var metadata = new Dictionary<string, object>
            {
                { "sagaId", msg.CorrelationId.ToString() }
            };

            await _mediator.Send(new ProcessarDebitoCommand
            {
                AccountId = msg.AccountId,
                Amount = msg.Amount,
                Currency = "BRL",
                ReferenceId = msg.ReferenceId,
                Metadata = metadata
            });
        }

        public async Task Consume(ConsumeContext<RealizarCreditoCommand> context)
        {
            var msg = context.Message;

            var metadata = new Dictionary<string, object>
            {
                { "sagaId", msg.CorrelationId.ToString() }
            };

            await _mediator.Send(new ProcessarCreditoCommand
            {
                AccountId = msg.AccountId,
                Amount = msg.Amount,
                Currency = "BRL",
                ReferenceId = msg.ReferenceId,
                Metadata = metadata
            });
        }

        public async Task Consume(ConsumeContext<RealizarEstornoCommand> context)
        {
            var msg = context.Message;

            var metadata = new Dictionary<string, object>
            {
                { "sagaId", msg.CorrelationId.ToString() },
                { "originalReferenceId", msg.OriginalReferenceId }
            };

            await _mediator.Send(new ProcessarEstornoCommand
            {
                AccountId = msg.AccountId,
                Amount = msg.Amount,
                Currency = "BRL",
                ReferenceId = msg.ReferenceId,
                Metadata = metadata
            });
        }
    }
}