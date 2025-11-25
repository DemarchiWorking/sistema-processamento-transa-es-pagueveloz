using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

using Microsoft.Extensions.Configuration.Json;


namespace PagueVeloz.CoreFinanceiro.Infra.Data
{
    public class CoreFinanceiroContextFactory : IDesignTimeDbContextFactory<CoreFinanceiroContext>
    {
        public CoreFinanceiroContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())

                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("FinanceiroConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'FinanceiroConnection' not found in appsettings.");
            }

            var builder = new DbContextOptionsBuilder<CoreFinanceiroContext>();

            var migrationsAssemblyName = typeof(CoreFinanceiroContextFactory).Assembly.GetName().Name;

            builder.UseNpgsql(connectionString, b =>
            {
                b.MigrationsAssembly(migrationsAssemblyName);
            });

            return new CoreFinanceiroContext(builder.Options);
        }
    }
}

//dotnet ef migrations add AdicionarTabelasMassTransit --project PagueVeloz.CoreFinanceiro.Infra.csproj --startup-project ..\PagueVeloz.CoreFinanceiro.Api\PagueVeloz.CoreFinanceiro.Api.csproj --context CoreFinanceiroContext
//dotnet ef database update --project PagueVeloz.CoreFinanceiro.Infra.csproj --startup-project ..\PagueVeloz.CoreFinanceiro.Api\PagueVeloz.CoreFinanceiro.Api.csproj --context CoreFinanceiroContext
//dotnet ef migrations add ContasMassTransitIniciais --project PagueVeloz.Contas.Infra.csproj --startup-project ..\PagueVeloz.Contas.Api\PagueVeloz.Contas.Api.csproj --context ContasDbContext
//dotnet ef database update --project PagueVeloz.Contas.Infra.csproj --startup-project ..\PagueVeloz.Contas.Api\PagueVeloz.Contas.Api.csproj --context ContasDbContext

