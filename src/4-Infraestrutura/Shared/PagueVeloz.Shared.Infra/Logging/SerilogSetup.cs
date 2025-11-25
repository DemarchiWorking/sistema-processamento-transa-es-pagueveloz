using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Shared.Infra.Logging
{
    public static class SerilogSetup
    {
        public static IHostBuilder AddSerilogLogging(this IHostBuilder builder, string serviceName)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Service", serviceName) 
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({Service}) {NewLine}{Exception}")
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("MassTransit", Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Information();
            });
        }
    }
}
