namespace Autodor.Modules.Invoicing.Domain.ValueObjects;

public record Contractor(
    string Name,
    string City,
    string Street,
    string NIP,
    string ZipCode,
    string Email
);