namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Represents an email address for a contractor in the automotive parts distribution system.
/// This value object encapsulates electronic communication details used for business correspondence,
/// order notifications, and system-generated alerts. Provides type safety and domain meaning to email handling.
/// </summary>
/// <param name="Value">The email address string. Should be validated for proper email format before creating the value object to ensure communication reliability.</param>
public record Email(string Value);