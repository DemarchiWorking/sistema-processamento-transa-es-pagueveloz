using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Infra.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoreFinanceiroContext _context;
        private readonly int _maxRetries; 
        private IDbContextTransaction? _currentTransaction;

        public class ConcurrencyException : DbUpdateConcurrencyException
        {
            public ConcurrencyException(string message, DbUpdateConcurrencyException innerException)
                : base(message, innerException) { }
        }
        public UnitOfWork(CoreFinanceiroContext context, int maxRetries = 3)
        {
            _context = context;
            _maxRetries = maxRetries;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<bool> CommitAsync()
        {
            for (int attempt = 0; attempt < _maxRetries; attempt++)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (attempt == _maxRetries - 1)
                    {
                        throw new ConcurrencyException("Falha ao salvar devido a conflito de concorrência após várias tentativas de retry.", ex);
                    }

                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }
                    await Task.Delay(100);
                }
            }
            return false;
        }
    }
}