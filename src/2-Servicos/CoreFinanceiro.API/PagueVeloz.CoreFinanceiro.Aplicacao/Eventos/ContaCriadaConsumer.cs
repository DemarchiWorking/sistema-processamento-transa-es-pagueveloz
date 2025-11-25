using MassTransit;
using Microsoft.Extensions.Logging;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.Eventos.Contas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Aplicacao.Eventos
{

    public class ContaCriadaConsumer : IConsumer<ContaCriadaEvent>
    {
        private readonly ILogger<ContaCriadaConsumer> _logger;
        private readonly IContaRepository _contaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContaCriadaConsumer(
            ILogger<ContaCriadaConsumer> logger,
            IContaRepository contaRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _contaRepository = contaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<ContaCriadaEvent> context)
        {
            var evento = context.Message;
            var cancellationToken = context.CancellationToken;

            _logger.LogInformation("Recebido ContaCriadaEvent para AccountId: {AccountId}", evento.AccountId);

            var contaExistente = await _contaRepository.GetByIdAsync(evento.AccountId, cancellationToken);
            if (contaExistente != null)
            {
                _logger.LogWarning("Conta {AccountId} já existe no Ledger. Evento duplicado ignorado.", evento.AccountId);
                return;
            }

            var moeda = "BRL";

            var novaConta = Conta.CriarNova(
                evento.AccountId,
                moeda,
                evento.LimiteDeCredito
            );

            if (evento.InitialBalance > 0)
            {
                novaConta.Credit(evento.InitialBalance, $"INIT-{evento.AccountId}");
            }

            _contaRepository.Add(novaConta);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Ledger da conta {AccountId} criado com sucesso.", novaConta.AccountId);
        }
    }
}