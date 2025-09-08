namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Represents a physical address for a contractor in the automotive parts distribution system.
/// This value object encapsulates location information used for shipping, billing, and business correspondence.
/// Follows the Domain-Driven Design principle of making implicit concepts explicit through value objects.
/// </summary>
/// <param name="Street">The street address including building number and street name. Used for precise delivery location identification.</param>
/// <param name="City">The city name where the contractor is located. Essential for regional business operations and logistics.</param>
/// <param name="ZipCode">The postal code for accurate mail delivery and geographic region identification within the distribution network.</param>
public record Address(string Street, string City, string ZipCode);