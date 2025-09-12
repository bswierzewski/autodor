namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Unique identifier for a contractor entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record ContractorId(Guid Value);