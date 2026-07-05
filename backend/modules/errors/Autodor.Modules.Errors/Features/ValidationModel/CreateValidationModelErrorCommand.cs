using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public class CreateValidationModelErrorCommand
{
    [FromQuery]
    public string? Name { get; set; }

    [FromQuery]
    public string? Email { get; set; }

    [FromQuery]
    public int Quantity { get; set; }
}
