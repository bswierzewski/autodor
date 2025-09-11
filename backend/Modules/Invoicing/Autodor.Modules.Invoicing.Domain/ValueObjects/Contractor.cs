namespace Autodor.Modules.Invoicing.Domain.ValueObjects;

/// <summary>
/// Represents contractor information specifically formatted for invoice purposes.
/// This record contains the essential contractor details needed to generate
/// an invoice in external invoicing systems. The data is derived from the
/// full Contractor aggregate in the Contractors module but simplified
/// for invoice generation requirements.
/// </summary>
/// <param name="Name">Business name of the contractor as it should appear on the invoice</param>
/// <param name="City">City where the contractor is located</param>
/// <param name="Street">Street address of the contractor</param>
/// <param name="NIP">Polish tax identification number (NIP - Numer Identyfikacji Podatkowej)</param>
/// <param name="ZipCode">Postal/ZIP code of the contractor's address</param>
/// <param name="Email">Email address for invoice delivery and communication</param>
public record Contractor(
    string Name,
    string City,
    string Street,
    string NIP,
    string ZipCode,
    string Email
);