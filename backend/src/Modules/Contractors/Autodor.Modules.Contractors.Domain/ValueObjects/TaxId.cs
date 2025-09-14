namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Tax identification number (NIP) for a contractor.
/// </summary>
/// <param name="Value">The tax ID value.</param>
public record TaxId(string Value);