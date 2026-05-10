using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.Forbidden;

public static class GetForbiddenErrorHandler
{
    [WolverineGet("/api/errors/forbidden")]
    [Tags("Errors")]
    [EndpointName("GetForbiddenError")]
    [EndpointSummary("Return a 403 problem details response")]
    public static string Handle()
    {
        throw new ForbiddenAccessException("This is a simulated forbidden error.");
    }
}