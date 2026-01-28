namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public record UpdateContractorCommand(
    string NIP,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
);
