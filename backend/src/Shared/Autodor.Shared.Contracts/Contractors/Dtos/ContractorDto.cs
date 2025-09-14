namespace Autodor.Shared.Contracts.Contractors.Dtos;

public record ContractorDto(
    Guid Id,
    string NIP,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
);