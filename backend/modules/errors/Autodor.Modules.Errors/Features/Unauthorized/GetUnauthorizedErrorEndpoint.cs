using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.Unauthorized;

public static class GetUnauthorizedErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/unauthorized", Handle)
            .RequireAuthorization()
            .WithTags("Errors")
            .WithName("GetUnauthorizedError")
            .WithSummary("Return a 401 problem details response");
    }

    private static Task<string> Handle(
        [AsParameters] GetUnauthorizedErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}