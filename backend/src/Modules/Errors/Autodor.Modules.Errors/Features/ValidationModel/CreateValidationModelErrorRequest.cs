using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Errors.Features.ValidationModel;

public class CreateValidationModelErrorRequest
{
    [FromQuery]
    public string? Name { get; set; }

    [FromQuery]
    public string? Email { get; set; }

    [FromQuery]
    public int Quantity { get; set; }
}