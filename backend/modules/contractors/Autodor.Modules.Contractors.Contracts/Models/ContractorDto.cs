namespace Autodor.Modules.Contractors.Contracts.Models;

/// <summary>
/// Data Transfer Object for Contractor entity
/// Used for inter-module communication
/// </summary>
public record ContractorDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email);
