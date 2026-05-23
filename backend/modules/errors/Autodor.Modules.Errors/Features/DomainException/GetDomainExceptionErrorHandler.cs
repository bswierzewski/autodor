using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.DomainException;

public static class GetDomainExceptionErrorHandler
{
    [Authorize]
    public static string Handle(GetDomainExceptionErrorCommand command)
    {
        throw new BuildingBlocks.Core.Exceptions.DomainException(
            "This is a simulated domain rule violation.");
    }
}