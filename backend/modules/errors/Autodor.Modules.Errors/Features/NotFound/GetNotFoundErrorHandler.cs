using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.NotFound;

public static class GetNotFoundErrorHandler
{
    [Authorize]
    public static string Handle(GetNotFoundErrorCommand command)
    {
        throw new NotFoundException("This is a simulated not found error.");
    }
}