namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Represents a tax identification number (NIP in Poland) for a contractor in the automotive parts distribution system.
/// This value object encapsulates the legal tax identifier required for business transactions, invoicing, and regulatory compliance.
/// Essential for VAT calculations, tax reporting, and ensuring legal business relationships with contractors.
/// </summary>
/// <param name="Value">The tax identification number as a string. Should follow the country-specific format (e.g., Polish NIP format) for regulatory compliance.</param>
public record TaxId(string Value);