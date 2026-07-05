using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public static class CreateValidationModelErrorEndpoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/errors/validation-model", Handle)
            .RequireAuthorization()
            .WithTags("Errors")
            .WithName("CreateValidationModelError")
            .WithSummary("Validate an AsParameters query model with FluentValidation and return problem details on failure");
    }

    private static Task<string> Handle(
        [AsParameters] CreateValidationModelErrorCommand command,
        IMessageBus bus,
        CancellationToken ct)
    {
        return bus.InvokeAsync<string>(command, ct);
    }
}
