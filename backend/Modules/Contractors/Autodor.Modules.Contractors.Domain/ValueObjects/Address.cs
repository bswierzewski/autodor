namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Physical address for a contractor.
/// </summary>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="ZipCode">Postal code.</param>
public record Address(string Street, string City, string ZipCode);