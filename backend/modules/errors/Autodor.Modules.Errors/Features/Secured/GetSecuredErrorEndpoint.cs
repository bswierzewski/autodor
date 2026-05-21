using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.Secured;

public static class GetSecuredErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/secured", Handle)
            .WithTags("Errors")
            .WithName("GetSecuredError")
            .WithSummary("Require a fake role-derived permission to demonstrate authorization errors");
    }

    private static Task<string> Handle(
        [AsParameters] GetSecuredErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}