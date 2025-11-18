using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PagueVeloz.Contas.Aplicacao.Interfaces;
using PagueVeloz.Contas.Infra.Data;
using PagueVeloz.Contas.Infra.Data.Repositories;
using PagueVeloz.Contas.Aplicacao.Comandos;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


builder.Services.AddOpenApi();

//#######################
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CriarContaCommand).Assembly);
});

builder.Services.AddDbContext<ContasDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("ContaConnection")));

services.AddScoped<IClienteRepository, ClienteRepository>();
services.AddScoped<IContaRepository, ContaRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();


//#######################
var app = builder.Build();

//configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
