using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Core.Primitives;

namespace Autodor.Modules.Errors;

public sealed class ErrorsRolePermissionProvider : IRolePermissionProvider
{
    public const string DebugRole = "errors-debugger";

    public IEnumerable<Role> GetRolePermissions()
    {
        yield return new Role(DebugRole, [ErrorsPermissions.AccessSecureEndpoint]);
    }
}
