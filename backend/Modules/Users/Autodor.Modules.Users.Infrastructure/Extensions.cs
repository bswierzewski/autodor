using Autodor.Modules.Users.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Users.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddUsersForWeb(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUser, JwtUserService>();

        return services;
    }

    public static IServiceCollection AddUsersForConsole(this IServiceCollection services)
    {
        services.AddScoped<IUser, StaticUserService>();

        return services;
    }
}