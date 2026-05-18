using BuildingBlocks.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Errors;

public sealed class ErrorsModule : IModule
{
    public string Name => "Errors";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRolePermissionProvider, ErrorsRolePermissionProvider>();
    }
}