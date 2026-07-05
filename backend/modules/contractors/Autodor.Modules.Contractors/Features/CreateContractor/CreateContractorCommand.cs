namespace Autodor.Modules.Contractors.Features.CreateContractor;

public record CreateContractorCommand(
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);

public record CreateContractorResponse(Guid Id);
