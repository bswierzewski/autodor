using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public static class CreateValidationModelErrorHandler
{
    [WolverineGet("/api/errors/validation-model")]
    [Tags("Errors")]
    [EndpointName("CreateValidationModelError")]
    [EndpointSummary("Validate an AsParameters query model with FluentValidation and return problem details on failure")]
    public static string Handle([AsParameters] CreateValidationModelErrorRequest request)
    {
        return $"Validation passed for {request.Name} ({request.Email}) with quantity {request.Quantity}.";
    }
}