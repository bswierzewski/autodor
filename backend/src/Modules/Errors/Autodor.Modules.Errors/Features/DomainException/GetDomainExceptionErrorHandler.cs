using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.DomainException;

public static class GetDomainExceptionErrorHandler
{
    [WolverineGet("/api/errors/domain-exception")]
    [Tags("Errors")]
    [EndpointName("GetDomainExceptionError")]
    [EndpointSummary("Return a 400 domain rule violation problem details response")]
    public static string Handle()
    {
        throw new BuildingBlocks.Core.Exceptions.DomainException(
            "This is a simulated domain rule violation.");
    }
}