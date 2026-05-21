using BuildingBlocks.Core.Exceptions;

namespace Autodor.Modules.Errors.Features.Forbidden;

public static class GetForbiddenErrorHandler
{
    public static string Handle(GetForbiddenErrorCommand command)
    {
        throw new ForbiddenAccessException("This is a simulated forbidden error.");
    }
}