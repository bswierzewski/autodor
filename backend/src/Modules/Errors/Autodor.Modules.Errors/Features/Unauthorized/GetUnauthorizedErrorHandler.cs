using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.Unauthorized;

public static class GetUnauthorizedErrorHandler
{
    [WolverineGet("/api/errors/unauthorized")]
    [Tags("Errors")]
    [EndpointName("GetUnauthorizedError")]
    [EndpointSummary("Return a 401 problem details response")]
    public static string Handle()
    {
        throw new UnauthorizedAccessException("This is a simulated unauthorized error.");
    }
}