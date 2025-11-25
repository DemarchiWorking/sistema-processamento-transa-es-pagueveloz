using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PagueVeloz.Contas.Aplicacao.Comandos;
using PagueVeloz.Contas.Aplicacao.Interfaces;
using PagueVeloz.Contas.Infra.Data;
using PagueVeloz.Contas.Infra.Data.Repositories;
using PagueVeloz.Shared.Infra.Logging;


var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilogLogging("Contas.API");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Contas API", Version = "v1" });
});
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var services = builder.Services;
var configuration = builder.Configuration;

builder.Services.AddOpenApi();


services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CriarContaCommand).Assembly);
});

builder.Services.AddDbContext<ContasDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("ContaConnection")));

services.AddScoped<IClienteRepository, ClienteRepository>();
services.AddScoped<IContaRepository, ContaRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddMassTransit(busConfig =>
{
    busConfig.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["MessageBroker:Host"], "/", h =>
        {
            h.Username(configuration["MessageBroker:Username"]);
            h.Password(configuration["MessageBroker:Password"]);
        });

        cfg.ConfigureEndpoints(context);
    });

    busConfig.AddEntityFrameworkOutbox<ContasDbContext>(o =>
    {
        o.UseBusOutbox();

        o.UsePostgres();

        o.QueryDelay = TimeSpan.FromSeconds(10);
    });
});

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = serviceProvider.GetRequiredService<ContasDbContext>();
            context.Database.Migrate();
            logger.LogInformation("Migrações aplicadas com sucesso.");
        }
        catch (Exception dbEx)
        {
            logger.LogError(dbEx, "Ocorreu um erro FATAL ao aplicar as migrações ao banco de dados.");

            throw;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Erro Durante as migracoes:" + ex);
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
