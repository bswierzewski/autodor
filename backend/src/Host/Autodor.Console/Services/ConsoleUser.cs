using BuildingBlocks.Abstractions.Abstractions;

namespace Autodor.Console.Services;

/// <summary>
/// Console application implementation of IUserContext.
/// Represents system operations performed by console application.
/// </summary>
public class ConsoleUser : IUserContext
{
    /// <summary>
    /// Returns Guid.Empty to indicate system operations in console context.
    /// </summary>
    public Guid Id => Guid.Empty;

    /// <summary>
    /// Console application has no roles.
    /// </summary>
    public IEnumerable<string> Roles => Enumerable.Empty<string>();

    /// <summary>
    /// Console application is always in any role (no authorization needed).
    /// </summary>
    public bool IsInRole(string role) => true;
}
