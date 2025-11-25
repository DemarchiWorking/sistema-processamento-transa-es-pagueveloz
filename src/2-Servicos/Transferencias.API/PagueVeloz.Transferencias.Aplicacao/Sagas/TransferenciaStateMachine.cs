using MassTransit;
using PagueVeloz.Eventos.Comandos;
using PagueVeloz.Eventos.Contas;
using PagueVeloz.Eventos.CoreFinanceiro;
using PagueVeloz.Eventos.Transferencias;
using PagueVeloz.Transferencias.Dominio.Sagas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Transferencias.Aplicacao.Sagas
{
    public record RealizarDebitoCommand(Guid CorrelationId, string AccountId, long Amount, string ReferenceId, string Currency);
    public record RealizarCreditoCommand(Guid CorrelationId, string AccountId, long Amount, string ReferenceId, string Currency);
    public record RealizarEstornoCommand(Guid CorrelationId, string AccountId, long Amount, string ReferenceId, string OriginalReferenceId, string Currency);

    public record TransferenciaSolicitadaEvent(Guid TransferenciaId, string ContaOrigemId, string ContaDestinoId, long Valor, string Currency);
    public record ContaDebitoRealizadoDomainEvent(string ReferenceId);
    public record ContaCreditoRealizadoDomainEvent(string ReferenceId); 
    public record ContaEstornoRealizadoDomainEvent(string OriginalReferenceId); 
    public record OperacaoFinanceiraFalhouEvent(string ReferenceId, string MensagemErro); 
    public record TransferenciaConcluidaEvent(Guid TransferenciaId, string Status, DateTime Timestamp, string? MotivoFalha = null);

    public class TransferenciaStateMachine : MassTransitStateMachine<TransferenciaState>
    {
        private static readonly Uri DebitoQueueUri = new Uri("queue:corefinanceiro_debito_queue");
        private static readonly Uri CreditoQueueUri = new Uri("queue:corefinanceiro_credito_queue");
        private static readonly Uri EstornoQueueUri = new Uri("queue:corefinanceiro_estorno_queue");

        public State DebitandoOrigem { get; private set; }
        public State CreditandoDestino { get; private set; }
        public State CompensandoDebito { get; private set; }
        public State FalhaCompensada { get; private set; }
        public State FalhaIrrecuperavel { get; private set; }

        public Event<TransferenciaSolicitadaEvent> TransferenciaSolicitada { get; private set; }
        public Event<ContaDebitoRealizadoDomainEvent> DebitoRealizado { get; private set; }
        public Event<ContaCreditoRealizadoDomainEvent> CreditoRealizado { get; private set; }
        public Event<ContaEstornoRealizadoDomainEvent> EstornoRealizado { get; private set; }
        public Event<OperacaoFinanceiraFalhouEvent> OperacaoFalhou { get; private set; }

        public Event<TransferenciaConcluidaEvent> TransferenciaConcluida { get; private set; }


        public TransferenciaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => TransferenciaSolicitada, x =>
            {
                x.CorrelateById(m => m.Message.TransferenciaId);
                x.SelectId(m => m.Message.TransferenciaId);
            });

            Event(() => DebitoRealizado, x => x.CorrelateById(m => Guid.Parse(m.Message.ReferenceId)));
            Event(() => CreditoRealizado, x => x.CorrelateById(m => Guid.Parse(m.Message.ReferenceId)));
            Event(() => OperacaoFalhou, x => x.CorrelateById(m => Guid.Parse(m.Message.ReferenceId)));

            Event(() => EstornoRealizado, x => x.CorrelateById(m => Guid.Parse(m.Message.OriginalReferenceId)));

            Event(() => TransferenciaConcluida, x => x.CorrelateById(m => m.Message.TransferenciaId));

            Initially(
                When(TransferenciaSolicitada)
                    .Then(ctx =>
                    {
                        ctx.Saga.AccountIdOrigem = ctx.Message.ContaOrigemId;
                        ctx.Saga.AccountIdDestino = ctx.Message.ContaDestinoId;
                        ctx.Saga.Amount = ctx.Message.Valor;
                        ctx.Saga.Currency = ctx.Message.Currency;
                        ctx.Saga.Timestamp = DateTime.UtcNow;
                    })
                    .TransitionTo(DebitandoOrigem)
                    .Send(DebitoQueueUri, ctx => new RealizarDebitoCommand(
                        CorrelationId: ctx.Saga.CorrelationId,
                        AccountId: ctx.Saga.AccountIdOrigem,
                        Amount: ctx.Saga.Amount,
                        ReferenceId: ctx.Saga.CorrelationId.ToString(),
                        Currency: ctx.Saga.Currency
                    ))
            );

            During(DebitandoOrigem,
                When(DebitoRealizado)
                    .TransitionTo(CreditandoDestino)
                    .Send(CreditoQueueUri, ctx => new RealizarCreditoCommand(
                        CorrelationId: ctx.Saga.CorrelationId,
                        AccountId: ctx.Saga.AccountIdDestino,
                        Amount: ctx.Saga.Amount,
                        ReferenceId: ctx.Saga.CorrelationId.ToString(),
                        Currency: ctx.Saga.Currency
                    )),

                When(OperacaoFalhou)
                    .Then(ctx => { ctx.Saga.MotivoFalha = $"Falha no Débito da Origem: {ctx.Message.MensagemErro}"; })
                    .Finalize()
            );


            During(CreditandoDestino,
                When(CreditoRealizado)
                    .Publish(ctx => new TransferenciaConcluidaEvent(
                        TransferenciaId: ctx.Saga.CorrelationId,
                        Status: "SUCCESS",
                        Timestamp: DateTime.UtcNow
                    ))
                    .Finalize(),

                When(OperacaoFalhou)
                    .TransitionTo(CompensandoDebito)
                    .Then(ctx => { ctx.Saga.MotivoFalha = $"Falha no Crédito do Destino: {ctx.Message.MensagemErro}"; })

                    .Send(EstornoQueueUri, ctx => new RealizarEstornoCommand(
                        CorrelationId: ctx.Saga.CorrelationId,
                        AccountId: ctx.Saga.AccountIdOrigem,
                        Amount: ctx.Saga.Amount,
                        ReferenceId: Guid.NewGuid().ToString(), 
                        OriginalReferenceId: ctx.Saga.CorrelationId.ToString(),
                        Currency: ctx.Saga.Currency
                    ))
            );
            During(CompensandoDebito,
                When(EstornoRealizado)
                    .Publish(ctx => new TransferenciaConcluidaEvent(
                        TransferenciaId: ctx.Saga.CorrelationId,
                        Status: "COMPENSATED_FAILURE",
                        MotivoFalha: ctx.Saga.MotivoFalha,
                        Timestamp: DateTime.UtcNow
                    ))
                    .TransitionTo(FalhaCompensada)
                    .Finalize(),

                When(OperacaoFalhou)
                    .Then(ctx =>
                    {
                        ctx.Saga.MotivoFalha += $"; Falha CRÍTICA no Estorno Compensatório (Rollback): {ctx.Message.MensagemErro}";
                    })
                    .TransitionTo(FalhaIrrecuperavel)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
