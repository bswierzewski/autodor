using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.BadRequest;

public static class GetBadRequestErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/bad-request", Handle)
            .RequireAuthorization()
            .WithTags("Errors")
            .WithName("GetBadRequestError")
            .WithSummary("Return a 400 validation problem details response");
    }

    private static Task<string> Handle(
        [AsParameters] GetBadRequestErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}
