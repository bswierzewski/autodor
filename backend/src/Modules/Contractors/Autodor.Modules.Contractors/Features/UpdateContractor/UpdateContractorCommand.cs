using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public record UpdateContractorCommand(
    [FromRoute] Guid Id,
    string NIP,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
);
