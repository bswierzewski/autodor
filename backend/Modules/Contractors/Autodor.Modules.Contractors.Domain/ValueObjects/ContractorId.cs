namespace Autodor.Modules.Contractors.Domain.ValueObjects;

/// <summary>
/// Represents a unique identifier for a contractor in the automotive parts distribution system.
/// This strongly-typed identifier prevents primitive obsession and ensures type safety when working with contractor references.
/// The GUID-based approach guarantees uniqueness across distributed systems and database partitions.
/// </summary>
/// <param name="Value">The unique GUID value that identifies a specific contractor. Generated automatically to ensure global uniqueness.</param>
public record ContractorId(Guid Value);