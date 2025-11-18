using MediatR;
using PagueVeloz.Contas.Aplicacao.Interfaces;
using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Comandos
{
    ///<summary>
    ///caso de Uso: Handler para o CriarContaCommand.
    ///orquestra a logica: validacao, execucao do dominio e persistencia.
    ///</summary>
    public class CriarContaCommandHandler : IRequestHandler<CriarContaCommand, CriarContaResponse>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IContaRepository _contaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarContaCommandHandler(
            IClienteRepository clienteRepository,
            IContaRepository contaRepository,
            IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _contaRepository = contaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CriarContaResponse> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            //validacao
            //verificamos se o cliente informado existe.
            if (!await _clienteRepository.ExisteAsync(request.ClientId, cancellationToken))
            {
                //alerta: usar uma exception customizada | result pattern | r etornar um erro 404/400.
                throw new ApplicationException($"Cliente com ID '{request.ClientId}' não encontrado.");
            }

            //execucao [regra de dominio]
            //chamar a fabrica do agregado para criar a conta.
            var conta = Conta.Criar(
                request.ClientId,
                request.CreditLimit
            );

            //persistencia [inicio da transacao]
            //adicionar o novo agregado ao repositorio.
            _contaRepository.Adicionar(conta);

            //criacao do evento integracao
            var evento = new ContaCriadaEvent(
                conta.Id,
                conta.ClienteId,
                conta.LimiteDeCredito,
                (PagueVeloz.Eventos.Contas.StatusConta)conta.Status,
                DateTime.UtcNow
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            //resposta
            return new CriarContaResponse(
                conta.Id,
                conta.ClienteId,
                conta.LimiteDeCredito,
                conta.Status.ToString(),
                DateTime.UtcNow //pegar do banco
            );
        }
    }
}