using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.NotFound;

public static class GetNotFoundErrorHandler
{
    [WolverineGet("/api/errors/not-found")]
    [Tags("Errors")]
    [EndpointName("GetNotFoundError")]
    [EndpointSummary("Return a 404 problem details response")]
    public static string Handle()
    {
        throw new NotFoundException("This is a simulated not found error.");
    }
}