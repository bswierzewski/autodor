using Autodor.Modules.Errors.Features.BadRequest;
using Autodor.Modules.Errors.Features.DomainException;
using Autodor.Modules.Errors.Features.Forbidden;
using Autodor.Modules.Errors.Features.InternalServerError;
using Autodor.Modules.Errors.Features.NotFound;
using Autodor.Modules.Errors.Features.Secured;
using Autodor.Modules.Errors.Features.Unauthorized;
using Autodor.Modules.Errors.Features.ValidationModel;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Errors;

public sealed class ErrorsModule : IModuleEndpoint
{
    public string Name => "Errors";

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        GetBadRequestErrorEndpoint.Map(endpoints);
        GetDomainExceptionErrorEndpoint.Map(endpoints);
        GetForbiddenErrorEndpoint.Map(endpoints);
        GetInternalServerErrorEndpoint.Map(endpoints);
        GetNotFoundErrorEndpoint.Map(endpoints);
        GetSecuredErrorEndpoint.Map(endpoints);
        GetUnauthorizedErrorEndpoint.Map(endpoints);
        CreateValidationModelErrorEndpoint.Map(endpoints);
    }

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRolePermissionProvider, ErrorsRolePermissionProvider>();
    }
}
