using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Options;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        services.AddMemoryCache();

        //Options
        services.Configure<PolcarOptions>(configuration.GetSection("Credentials:Polcar"));
        services.Configure<IFirmaOptions>(configuration.GetSection("Credentials:IFirma"));
        services.Configure<InFaktOptions>(configuration.GetSection("Credentials:InFakt"));
        services.Configure<InvoiceProviderOptions>(configuration.GetSection("InvoiceProvider"));
        services.Configure<EmailOptions>(configuration.GetSection("Email"));

        return services;
    }
}
