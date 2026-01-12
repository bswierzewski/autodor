namespace Autodor.Shared.Core.Interfaces;

/// <summary>
/// Marker interface for strongly-typed configuration options for database connection
/// </summary>
public interface IDatabaseOptions : IOptions
{
    string ConnectionString { get; set; }
}