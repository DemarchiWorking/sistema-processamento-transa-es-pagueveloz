using PagueVeloz.CoreFinanceiro.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Api
{
    public class MigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationHostedService> _logger;

        public MigrationHostedService(IServiceProvider serviceProvider, ILogger<MigrationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando o serviço de aplicação de migrações...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CoreFinanceiroContext>();

                try
                {
                    await context.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Migrações aplicadas com sucesso. O banco de dados está pronto.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERRO FATAL: Falha ao aplicar as migrações.");

                    // throw; 
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}