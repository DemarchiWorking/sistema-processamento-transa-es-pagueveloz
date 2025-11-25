using Microsoft.EntityFrameworkCore;
using PagueVeloz.CoreFinanceiro.Dominio.DTOs;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Enums;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.CoreFinanceiro.Infra.Persistencia;
using PagueVeloz.Eventos.CoreFinanceiro;
using PagueVeloz.Eventos.Transferencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly CoreFinanceiroContext _context;

        public ContaRepository(CoreFinanceiroContext context)
        {
            _context = context;
        }

        public async Task<Conta> GetByIdAsync(string accountId, CancellationToken cancellationToken = default)
        {
            return await _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefaultAsync(c => c.AccountId == accountId, cancellationToken);
        }
        public async Task<Conta> GetByAccountNumber(string numeroConta, CancellationToken cancellationToken)
        {
            return await _context.Contas
               .Include(c => c.Transacoes)
               .FirstOrDefaultAsync(c => c.AccountId == numeroConta, cancellationToken);
        }
        public async Task<Movimento> GetMovimentoByIdAsync(string movimentoId, CancellationToken cancellationToken)
        {
            return await _context.Movimentos
               .FirstOrDefaultAsync(m => m.Id == movimentoId, cancellationToken);
        }

        public void Add(Conta conta)
        {
            _context.Contas.Add(conta);
        }
        public void Update(Conta conta)
        {
            _context.Contas.Update(conta);
        }
        public async Task<TransacaoProcessada> GetAsynchronousProcessedTransaction(string referenceId)
        {
            return await _context.TransacoesProcessadas.FindAsync(referenceId);
        }

        public void RegisterTransactionProcessed(TransacaoProcessada transacao)
        {
            _context.TransacoesProcessadas.Add(transacao);
        }
    }
}