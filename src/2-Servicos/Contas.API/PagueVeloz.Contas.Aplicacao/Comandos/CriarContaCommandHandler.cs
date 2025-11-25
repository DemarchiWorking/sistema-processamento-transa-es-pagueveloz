using MassTransit;
using MediatR;
using PagueVeloz.Contas.Aplicacao.Interfaces;
using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PagueVeloz.Eventos.Contas;

namespace PagueVeloz.Contas.Aplicacao.Comandos
{
    public class CriarContaCommandHandler : IRequestHandler<CriarContaCommand, CriarContaResponse>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IContaRepository _contaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint; 

        public CriarContaCommandHandler(
            IClienteRepository clienteRepository,
            IContaRepository contaRepository,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint
            )
        {
            _clienteRepository = clienteRepository;
            _contaRepository = contaRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint; 
        }

        public async Task<CriarContaResponse> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            if (!await _clienteRepository.ExisteAsync(request.ClientId, cancellationToken))
            {
                throw new ApplicationException($"Cliente com ID '{request.ClientId}' não encontrado.");
            }
            var id = _contaRepository.ObterProximoNumeroContaAsync();
            var conta = Conta.Criar(
                id.Result,
                request.ClientId,
                request.CreditLimit
            );

            _contaRepository.Add(conta);

            var evento = new ContaCriadaEvent(
                conta.Id,
                conta.ClienteId,
                conta.LimiteDeCredito,
                (PagueVeloz.Eventos.Contas.StatusConta)conta.Status,
                DateTime.UtcNow,
                request.InitialBalance
            );

            await _publishEndpoint.Publish(evento, cancellationToken); 

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CriarContaResponse(
                conta.Id,
                conta.ClienteId,
                conta.LimiteDeCredito,
                conta.Status.ToString(),
                DateTime.UtcNow 
            );
        }
    }
}