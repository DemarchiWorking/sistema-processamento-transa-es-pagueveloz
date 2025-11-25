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
    public class CriarClienteCommandHandler : IRequestHandler<CriarClienteCommand, CriarClienteResponse>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarClienteCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CriarClienteResponse> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
        {
            if (await _clienteRepository.ExisteAsync(request.ClienteId, cancellationToken))
            {
                throw new ApplicationException($"Cliente com ID '{request.ClienteId}' já existe.");
            }

            var cliente = Cliente.Criar(request.ClienteId, request.Nome); 

            _clienteRepository.Adicionar(cliente);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CriarClienteResponse(
                cliente.Id,
                cliente.Nome,
                DateTime.UtcNow
            );
        }
    }
}