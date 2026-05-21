namespace Autodor.Modules.Errors.Features.ValidationModel;

public static class CreateValidationModelErrorHandler
{
    public static string Handle(CreateValidationModelErrorCommand command)
    {
        return $"Validation passed for {command.Name} ({command.Email}) with quantity {command.Quantity}.";
    }
}