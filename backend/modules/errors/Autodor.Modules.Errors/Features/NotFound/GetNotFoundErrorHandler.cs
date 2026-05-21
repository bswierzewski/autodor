using BuildingBlocks.Core.Exceptions;

namespace Autodor.Modules.Errors.Features.NotFound;

public static class GetNotFoundErrorHandler
{
    public static string Handle(GetNotFoundErrorCommand command)
    {
        throw new NotFoundException("This is a simulated not found error.");
    }
}