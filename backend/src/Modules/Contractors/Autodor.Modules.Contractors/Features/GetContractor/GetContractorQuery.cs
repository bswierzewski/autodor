using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.GetContractor;

public record GetContractorQuery(
    [FromRoute] Guid Id
);

public record GetContractorResponse(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
