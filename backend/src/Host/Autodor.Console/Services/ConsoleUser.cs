using BuildingBlocks.Application.Abstractions;

namespace Autodor.Console.Services;

/// <summary>
/// Console application implementation of IUser.
/// Represents system operations performed by console application.
/// Returns null for Id to indicate system-created entities in audit fields.
/// </summary>
public class ConsoleUser : IUser
{
    /// <summary>
    /// Returns null to indicate system-created/modified entities.
    /// Null in audit fields (CreatedBy/ModifiedBy) indicates system operations.
    /// </summary>
    public Guid? Id => null;

    /// <summary>
    /// Console application has no email.
    /// </summary>
    public string? Email => null;

    /// <summary>
    /// Console application identifier for logging purposes.
    /// </summary>
    public string? FullName => "Console Application";

    /// <summary>
    /// Console application has no picture URL.
    /// </summary>
    public string? PictureUrl => null;

    /// <summary>
    /// Console application is not authenticated (no JWT token).
    /// </summary>
    public bool IsAuthenticated => false;

    /// <summary>
    /// Console application has no claims.
    /// </summary>
    public IEnumerable<string> Claims => Enumerable.Empty<string>();

    /// <summary>
    /// Console application has no roles.
    /// </summary>
    public IEnumerable<string> Roles => Enumerable.Empty<string>();

    /// <summary>
    /// Console application has no permissions.
    /// </summary>
    public IEnumerable<string> Permissions => Enumerable.Empty<string>();

    /// <summary>
    /// Console application is not in any role.
    /// </summary>
    public bool IsInRole(string role) => true;

    /// <summary>
    /// Console application has no claims.
    /// </summary>
    public bool HasClaim(string claimType, string? claimValue = null) => true;

    /// <summary>
    /// Console application has no permissions.
    /// </summary>
    public bool HasPermission(string permission) => true;
}
