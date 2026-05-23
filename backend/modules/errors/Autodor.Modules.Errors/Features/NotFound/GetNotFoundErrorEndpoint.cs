using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.NotFound;

public static class GetNotFoundErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/not-found", Handle)
            .RequireAuthorization()
            .WithTags("Errors")
            .WithName("GetNotFoundError")
            .WithSummary("Return a 404 problem details response");
    }

    private static Task<string> Handle(
        [AsParameters] GetNotFoundErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}