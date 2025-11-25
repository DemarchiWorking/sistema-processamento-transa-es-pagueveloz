using MassTransit;
using Microsoft.EntityFrameworkCore;
using PagueVeloz.Transferencias.Aplicacao.Sagas;
using PagueVeloz.Transferencias.Dominio.Sagas;
using PagueVeloz.Transferencias.Infra.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TransferenciasDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("TransferenciaConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<TransferenciaStateMachine, TransferenciaState>()
     .EntityFrameworkRepository(r =>
     {
         r.ConcurrencyMode = ConcurrencyMode.Optimistic;
         r.AddDbContext<DbContext, TransferenciasDbContext>();
     });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["MessageBroker:Host"], "/", h =>
        {
            h.Username(configuration["MessageBroker:Username"]);
            h.Password(configuration["MessageBroker:Password"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
