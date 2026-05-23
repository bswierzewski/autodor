using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.DomainException;

public static class GetDomainExceptionErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/domain-exception", Handle)
            .RequireAuthorization()
            .WithTags("Errors")
            .WithName("GetDomainExceptionError")
            .WithSummary("Return a 400 domain rule violation problem details response");
    }

    private static Task<string> Handle(
        [AsParameters] GetDomainExceptionErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}