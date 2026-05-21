using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.Secured;

public static class GetSecuredErrorHandler
{
    [Authorize(Permissions = [ErrorsPermissions.DebugSecureEndpoint])]
    public static string Handle(GetSecuredErrorCommand command)
    {
        return "You have the errors-debugger role and the required permission.";
    }
}