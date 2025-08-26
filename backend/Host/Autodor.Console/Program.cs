using Autodor.Console;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

var configuration = Extensions.CreateConfiguration();

using IHost host = configuration.CreateHostBuilder().Build();

Extensions.ConfigureLogging();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var options = services.GetRequiredService<IOptions<Options>>();
    var mediator = services.GetRequiredService<ISender>();

    // Przykład wywołania konkretnej operacji
    Log.Information("Console application started with options: {From} - {To}", options.Value.From, options.Value.To);

    // Tutaj można dodać konkretne komendy MediatR
    // await mediator.Send(new SomeCommand());
}
catch (Exception e)
{
    Log.Logger.Error(e, e.Message);
}