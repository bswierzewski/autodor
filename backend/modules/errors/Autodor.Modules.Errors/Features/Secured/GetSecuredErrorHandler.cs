using BuildingBlocks.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.Secured;

public static class GetSecuredErrorHandler
{
    [Authorize(Permissions = [ErrorsPermissions.DebugSecureEndpoint])]
    [WolverineGet("/api/errors/secured")]
    [Tags("Errors")]
    [EndpointName("GetSecuredError")]
    [EndpointSummary("Require a fake role-derived permission to demonstrate authorization errors")]
    public static string Handle()
    {
        return "You have the errors-debugger role and the required permission.";
    }
}