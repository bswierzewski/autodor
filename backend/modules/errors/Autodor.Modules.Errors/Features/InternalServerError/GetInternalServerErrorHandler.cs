using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.InternalServerError;

public static class GetInternalServerErrorHandler
{
    [Authorize]
    public static string Handle(GetInternalServerErrorCommand command)
    {
        throw new InvalidOperationException("To jest symulowany błąd wewnętrzny serwera.");
    }
}