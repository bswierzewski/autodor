using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.Forbidden;

public static class GetForbiddenErrorHandler
{
    [Authorize]
    public static string Handle(GetForbiddenErrorCommand command)
    {
        throw new ForbiddenAccessException("To jest symulowany błąd braku dostępu.");
    }
}