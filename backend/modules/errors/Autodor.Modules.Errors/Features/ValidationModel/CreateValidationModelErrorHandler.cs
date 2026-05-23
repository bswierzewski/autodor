using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public static class CreateValidationModelErrorHandler
{
    [Authorize]
    public static string Handle(CreateValidationModelErrorCommand command)
    {
        return $"Validation passed for {command.Name} ({command.Email}) with quantity {command.Quantity}.";
    }
}