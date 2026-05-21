using BuildingBlocks.Core.Exceptions;

namespace Autodor.Modules.Errors.Features.DomainException;

public static class GetDomainExceptionErrorHandler
{
    public static string Handle(GetDomainExceptionErrorCommand command)
    {
        throw new BuildingBlocks.Core.Exceptions.DomainException(
            "This is a simulated domain rule violation.");
    }
}