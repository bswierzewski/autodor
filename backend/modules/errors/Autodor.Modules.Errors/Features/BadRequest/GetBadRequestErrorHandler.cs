using BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.BadRequest;

public static class GetBadRequestErrorHandler
{
    [WolverineGet("/api/errors/bad-request")]
    [Tags("Errors")]
    [EndpointName("GetBadRequestError")]
    [EndpointSummary("Return a 400 validation problem details response")]
    public static string Handle()
    {
        throw new ValidationException(new Dictionary<string, string[]>
        {
            ["request"] = ["This is a simulated bad request error."]
        });
    }
}