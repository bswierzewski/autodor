using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Products.Domain.Aggregates;

/// <summary>
/// Represents the state of a background task, providing persistent tracking of execution history.
/// </summary>
public class BackgroundTaskState : AggregateRoot<int>
{
    /// <summary>
    /// Gets or sets the unique name identifier for the background task.
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the task was last executed.
    /// </summary>
    public DateTime LastExecutedAt { get; set; }

    /// <summary>
    /// Gets or sets additional JSON data for task-specific metadata.
    /// </summary>
    public string? AdditionalData { get; set; }
}