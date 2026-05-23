using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.InternalServerError;

public static class GetInternalServerErrorHandler
{
    [Authorize]
    public static string Handle(GetInternalServerErrorCommand command)
    {
        throw new InvalidOperationException("This is a simulated internal server error.");
    }
}