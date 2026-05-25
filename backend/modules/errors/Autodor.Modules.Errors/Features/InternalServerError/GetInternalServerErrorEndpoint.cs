using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.InternalServerError;

public static class GetInternalServerErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/internal-server-error", Handle)
            .RequireAuthorization()
            .WithTags("Errors")
            .WithName("GetInternalServerError")
            .WithSummary("Return a 500 problem details response");
    }

    private static Task<string> Handle(
        [AsParameters] GetInternalServerErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}