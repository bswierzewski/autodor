using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.InternalServerError;

public static class GetInternalServerErrorHandler
{
    [WolverineGet("/api/errors/internal-server-error")]
    [Tags("Errors")]
    [EndpointName("GetInternalServerError")]
    [EndpointSummary("Return a 500 problem details response")]
    public static string Handle()
    {
        throw new InvalidOperationException("This is a simulated internal server error.");
    }
}