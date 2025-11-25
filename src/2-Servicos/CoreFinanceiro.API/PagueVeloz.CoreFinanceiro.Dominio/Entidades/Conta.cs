using PagueVeloz.CoreFinanceiro.Dominio.Enums;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.Eventos.Contas;
using PagueVeloz.Eventos.CoreFinanceiro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.Entidades
{

    public class Conta : AggregateRoot
    {
        public string AccountId { get; private set; }
        public string Currency { get; private set; }
        public string Status { get; private set; } 

        public long SaldoDisponivelEmCentavos { get; private set; }
        public long SaldoReservadoEmCentavos { get; private set; }
        public long LimiteDeCreditoEmCentavos { get; private set; }
        public uint LockVersion { get; private set; } = 1;

        public long SaldoTotal => SaldoDisponivelEmCentavos + SaldoReservadoEmCentavos;
        public long PoderDeCompra => SaldoDisponivelEmCentavos + LimiteDeCreditoEmCentavos;

        private readonly List<Transacao> _transacoes = new();
        public IReadOnlyCollection<Transacao> Transacoes => _transacoes.AsReadOnly();

        private Conta() { }

        public static Conta CriarNova(string accountId, string currency, long limiteInicial = 50000)
        {
            if (string.IsNullOrWhiteSpace(accountId)) throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentNullException(nameof(currency));
            if (limiteInicial < 0) throw new ArgumentException("Limite não pode ser negativo", nameof(limiteInicial));

            return new Conta
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Currency = currency,
                Status = "active",
                LimiteDeCreditoEmCentavos = limiteInicial,
                SaldoDisponivelEmCentavos = 0,
                SaldoReservadoEmCentavos = 0,
                LockVersion = 1
            };
        }

        public void Debit(long valor, string referenceId)
        {
            ValidarStatusEOperacao(valor);

            if (_transacoes.Any(t => t.ReferenceId == referenceId))
            {
                return; //throw new TransacaoJaProcessadaException(referenceId);
            }

            if (valor > PoderDeCompra)
            {
                return;//throw new SaldoInsuficienteException(AccountId, valor, PoderDeCompra);
            }

            SaldoDisponivelEmCentavos -= valor;

            var transacao = RegistrarAlteracao(TipoTransacao.Debit, valor, referenceId);

            AddDomainEvent(new ContaDebitoRealizadoDomainEvent(
                TransactionId: transacao.Id,
                AccountId: this.AccountId,
                Valor: valor,
                Currency: this.Currency,
                ReferenceId: referenceId,
                SaldoDisponivelEmCentavos: SaldoDisponivelEmCentavos,
                SaldoReservadoEmCentavos: SaldoReservadoEmCentavos,
                LimiteDeCreditoEmCentavos: LimiteDeCreditoEmCentavos,
                Timestamp: DateTime.UtcNow
            ));
        }

        public void Credit(long valor, string referenceId)
        {
            ValidarStatusEOperacao(valor);

            if (_transacoes.Any(t => t.ReferenceId == referenceId))
            {
                throw new TransacaoJaProcessadaException(referenceId);
            }

            SaldoDisponivelEmCentavos += valor;

            var transacao = RegistrarAlteracao(TipoTransacao.Credit, valor, referenceId);

            AddDomainEvent(new ContaCreditoRealizadoDomainEvent(
                transactionId: transacao.Id,
                referenceId: referenceId,
                valor: valor,
                accountId: this.AccountId,
                currency: this.Currency,
                saldoDisponivelEmCentavos: SaldoDisponivelEmCentavos,
                saldoReservadoEmCentavos: SaldoReservadoEmCentavos,
                limiteDeCreditoEmCentavos: LimiteDeCreditoEmCentavos,
                timestamp: DateTime.UtcNow
            ));
        }

        public void Reserve(long amount, string referenceId)
        {
            ValidarStatusEOperacao(amount);

            if (_transacoes.Any(t => t.ReferenceId == referenceId))
            {
                throw new TransacaoJaProcessadaException(referenceId);
            }

            if (amount > SaldoDisponivelEmCentavos)
            {
                throw new SaldoInsuficienteException(AccountId, amount, SaldoDisponivelEmCentavos);
            }

            SaldoDisponivelEmCentavos -= amount;
            SaldoReservadoEmCentavos += amount;

            RegistrarAlteracao(TipoTransacao.Reserve, amount, referenceId);

        }

        public void Capture(long amount, string referenceId)
        {
            ValidarStatusEOperacao(amount);

            if (_transacoes.Any(t => t.ReferenceId == referenceId))
            {
                throw new TransacaoJaProcessadaException(referenceId);
            }

            if (amount > SaldoReservadoEmCentavos)
            {
                throw new DomainException($"Saldo reservado insuficiente. Valor solicitado: {amount}. Saldo disponível para captura: {SaldoReservadoEmCentavos}.");
            }

            SaldoReservadoEmCentavos -= amount;

            RegistrarAlteracao(TipoTransacao.Capture, amount, referenceId);

        }

        public void Estornar(string reversalReferenceId, string originalReferenceId)
        {
            ValidarStatusEOperacao(1);

            if (_transacoes.Any(t => t.ReferenceId == reversalReferenceId))
            {
                return;
            }

            var transacaoOriginal = _transacoes
                .FirstOrDefault(t => t.ReferenceId == originalReferenceId)
                ?? throw new DomainException($"Transação original com ReferenceId '{originalReferenceId}' não encontrada no histórico.");

            if (transacaoOriginal.IsReversed)
            {
                if (transacaoOriginal.ReversalReferenceId == reversalReferenceId)
                {
                    return;
                }
                else
                {
                    throw new DomainException($"Transação {originalReferenceId} já estornada pela transação {transacaoOriginal.ReversalReferenceId}.");
                }
            }

            long valorAEstornar = transacaoOriginal.Valor;
            TipoTransacao tipoCompensacao;

            switch (transacaoOriginal.Tipo)
            {
                case TipoTransacao.Debit:
                case TipoTransacao.Capture:
                    tipoCompensacao = TipoTransacao.Credit;
                    SaldoDisponivelEmCentavos += valorAEstornar;
                    break;

                case TipoTransacao.Credit:
                    if (SaldoDisponivelEmCentavos < valorAEstornar)
                    {
                        throw new DomainException("Saldo disponível insuficiente para reverter o crédito (estorno).");
                    }
                    tipoCompensacao = TipoTransacao.Debit;
                    SaldoDisponivelEmCentavos -= valorAEstornar;
                    break;

                case TipoTransacao.Reserve:
                    if (SaldoReservadoEmCentavos < valorAEstornar)
                    {
                        throw new DomainException("Saldo reservado insuficiente para estornar a reserva original.");
                    }
                    tipoCompensacao = TipoTransacao.Release;
                    SaldoReservadoEmCentavos -= valorAEstornar;
                    SaldoDisponivelEmCentavos += valorAEstornar;
                    break;

                case TipoTransacao.Release:
                default:
                    throw new DomainException($"O tipo de transação {transacaoOriginal.Tipo} não pode ser estornado diretamente.");
            }

            var transacaoCompensatoria = RegistrarAlteracao(tipoCompensacao, valorAEstornar, reversalReferenceId);

            AddDomainEvent(new ContaEstornoRealizadoDomainEvent(
                TransactionId: transacaoCompensatoria.Id,
                AccountId: this.AccountId,
                Amount: valorAEstornar,
                Currency: this.Currency,
                ReferenceId: reversalReferenceId,
                OriginalReferenceId: originalReferenceId,
                Balance: SaldoTotal,
                ReservedBalance: SaldoReservadoEmCentavos,
                AvailableBalance: SaldoDisponivelEmCentavos,
                Timestamp: DateTime.UtcNow
            ));

            transacaoOriginal.MarcarComoEstornada(reversalReferenceId);
        }

        public void Block(string reason = "Solicitação interna")
        {
            if (Status == "blocked") return;
            Status = "blocked";
        }

        private void ValidarStatusEOperacao(long amount)
        {
            if (Status != "active")
            {
                throw new DomainException($"A conta {AccountId} está inativa ou bloqueada ({Status}) e não pode realizar operações.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("O valor da operação deve ser positivo.", nameof(amount));
            }
        }
        private Transacao RegistrarAlteracao(TipoTransacao tipo, long valor, string referenceId)
        {
            var tx = new Transacao(
                contaId: AccountId,
                tipo: tipo,
                valor: valor,
                referenceId: referenceId,
                currency: this.Currency,
                status: "success", 
                finalBalance: SaldoTotal,
                finalReservedBalance: SaldoReservadoEmCentavos,
                finalAvailableBalance: SaldoDisponivelEmCentavos
            );

            _transacoes.Add(tx);

            var contaAlteradaEvent = new ContaAlteradaEvent(
                accountId: AccountId,
                tipoOperacao: tipo.ToString(),
                valor: valor,
                referenceId: referenceId,
                saldoDisponivel: SaldoDisponivelEmCentavos,
                saldoReservado: SaldoReservadoEmCentavos
            );

            AddDomainEvent(contaAlteradaEvent);

            LockVersion++;

            return tx;
        }
        /*
        public void ProcessDebit(long amount, string referenceId)
        {
            ValidarStatusEOperacao(amount);

            if (_transacoes.Any(t => t.ReferenceId == referenceId))
            {
                throw new TransacaoJaProcessadaException(referenceId);
            }

            if (amount > PoderDeCompra)
            {
                throw new SaldoInsuficienteException(
                    AccountId,
                    amount,
                    PoderDeCompra
                );
            }

            long valorRestanteDoDebito = amount;

            if (valorRestanteDoDebito <= SaldoDisponivelEmCentavos)
            {
                SaldoDisponivelEmCentavos -= valorRestanteDoDebito;
                valorRestanteDoDebito = 0;
            }
            else
            {
                valorRestanteDoDebito -= SaldoDisponivelEmCentavos;
                SaldoDisponivelEmCentavos = 0;

                SaldoDisponivelEmCentavos -= valorRestanteDoDebito;
            }

            var transacao = RegistrarAlteracao(TipoTransacao.Debit, amount, referenceId);

            AddDomainEvent(new ContaDebitoRealizadoDomainEvent(
                TransactionId: transacao.Id,
                ReferenceId: referenceId,
                Valor: amount,
                AccountId: this.AccountId,
                Currency: this.Currency,
                SaldoDisponivelEmCentavos: SaldoDisponivelEmCentavos,
                SaldoReservadoEmCentavos: SaldoReservadoEmCentavos,
                LimiteDeCreditoEmCentavos: LimiteDeCreditoEmCentavos,
                Timestamp: DateTime.UtcNow
            ));
        }*/
    }
}

