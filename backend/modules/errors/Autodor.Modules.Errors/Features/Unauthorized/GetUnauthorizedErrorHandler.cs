using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.Unauthorized;

public static class GetUnauthorizedErrorHandler
{
    [Authorize]
    public static string Handle(GetUnauthorizedErrorCommand command)
    {
        throw new UnauthorizedAccessException("This is a simulated unauthorized error.");
    }
}