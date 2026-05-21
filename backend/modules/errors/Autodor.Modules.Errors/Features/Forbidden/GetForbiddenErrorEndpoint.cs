using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.Forbidden;

public static class GetForbiddenErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/forbidden", Handle)
            .WithTags("Errors")
            .WithName("GetForbiddenError")
            .WithSummary("Return a 403 problem details response");
    }

    private static Task<string> Handle(
        [AsParameters] GetForbiddenErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}