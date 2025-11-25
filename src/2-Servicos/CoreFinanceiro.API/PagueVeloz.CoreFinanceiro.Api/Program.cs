 using MassTransit;
using Microsoft.EntityFrameworkCore;
using PagueVeloz.CoreFinanceiro.Api;
using PagueVeloz.CoreFinanceiro.Aplicacao;
using PagueVeloz.CoreFinanceiro.Aplicacao.Eventos; //consumer
using PagueVeloz.CoreFinanceiro.Aplicacao.Projetores;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Interfaces;
using PagueVeloz.CoreFinanceiro.Infra.Data;
using PagueVeloz.CoreFinanceiro.Infra.Data.Repositories;
using PagueVeloz.CoreFinanceiro.Infra.Data.Servicos;
using PagueVeloz.CoreFinanceiro.Infra.Worker.PagueVeloz.CoreFinanceiro.Infra.Workers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
services.AddHostedService<MigrationHostedService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Core Financeiro API", Version = "v1" });
});

services.AddControllers();

services.AddDbContext<CoreFinanceiroContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("FinanceiroConnection")));

builder.Services.AddOpenApi();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<ITransacaoProcessadaRepository, TransacaoProcessadaRepository>();
builder.Services.AddSingleton<IMessagePublisher, MockMessagePublisher>();
builder.Services.AddSingleton<IMessageBrokerPublisher, MockMessagePublisher>();

builder.Services.AddHostedService<OutboxProcessorWorker>();
builder.Services.AddHostedService<OutboxMessageProcessor>();
builder.Services.AddScoped<IDomainEventService, DomainEventService>();


services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
{
    var context = provider.GetRequiredService<CoreFinanceiroContext>();
    const int DefaultRetries = 3; 
    return new UnitOfWork(context, DefaultRetries);
});
builder.Services.AddScoped<IProjetorRepository<TransacaoProjecao>, ProjetorRepository<TransacaoProjecao>>();


services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly)); 

services.AddMassTransit(busConfig =>
{
    busConfig.AddConsumer<ContaCriadaConsumer>();
    busConfig.AddConsumer<ContaTransacaoProjetor>();

    busConfig.AddEntityFrameworkOutbox<CoreFinanceiroContext>(o =>
    {
        o.UseBusOutbox();
        o.UsePostgres();
    });

    busConfig.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration["MessageBroker:Host"], "/", h =>
        {
            h.Username(configuration["MessageBroker:Username"]);
            h.Password(configuration["MessageBroker:Password"]);
        });
        cfg.ReceiveEndpoint("corefinanceiro-conta-criada", e =>
        {
            e.UseEntityFrameworkOutbox<CoreFinanceiroContext>(context);
            e.ConfigureConsumer<ContaCriadaConsumer>(context);
        });

        cfg.ReceiveEndpoint("corefinanceiro-transacao-projetor", e =>
        {
          

            e.ConfigureConsumer<ContaTransacaoProjetor>(context);
            e.UseMessageRetry(r =>
                r.Interval(3, TimeSpan.FromSeconds(5))
            );
        });
    });
});



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.MapControllers();
app.Run();

