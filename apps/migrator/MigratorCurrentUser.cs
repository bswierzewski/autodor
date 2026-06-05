using BuildingBlocks.Core.Interfaces;

namespace Autodor.Migrator;

/// <summary>
/// Migrator implementation of ICurrentUser used during database migrations when no user context is available.
/// </summary>
public sealed class MigratorCurrentUser : ICurrentUser
{
    public string Id => "Migrator";

    public bool IsAuthenticated => false;

    public IReadOnlySet<string> Roles => new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> Permissions => new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public bool HasPermission(string permission) => false;
}
