namespace Autodor.Modules.Contractors.Features.GetContractors;

public record GetContractorsResponse(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
