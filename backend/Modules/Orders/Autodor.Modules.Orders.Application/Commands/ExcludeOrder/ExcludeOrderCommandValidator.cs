using FluentValidation;

namespace Autodor.Modules.Orders.Application.Commands.ExcludeOrder;

/// <summary>
/// Validates ExcludeOrderCommand instances to ensure business rule compliance.
/// This validator enforces data integrity requirements before order exclusion processing.
/// Prevents invalid commands from reaching the domain layer and causing business logic errors.
/// </summary>
public class ExcludeOrderCommandValidator : AbstractValidator<ExcludeOrderCommand>
{
    /// <summary>
    /// Initializes validation rules for the ExcludeOrderCommand.
    /// Sets up business-critical validation to prevent processing of malformed exclusion requests.
    /// </summary>
    public ExcludeOrderCommandValidator()
    {
        // Ensure OrderId is provided as it's essential for identifying the order to exclude
        // Without a valid OrderId, the exclusion operation cannot proceed safely
        RuleFor(o => o.OrderId).NotEmpty()
            .WithMessage("OrderId is required for order exclusion operations");
    }
}