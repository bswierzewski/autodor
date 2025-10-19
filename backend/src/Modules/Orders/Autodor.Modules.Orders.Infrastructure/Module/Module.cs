using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Orders.Infrastructure.Module;

public class Module : IModule
{
    public string ModuleName => "Orders";
    public string DisplayName => "Zarządzanie Zamówieniami";
    public string? Description => "Moduł zarządzania zamówieniami i dokumentami magazynowymi";

    public IEnumerable<Permission> GetPermissions()
    {
        // Empty implementation - permissions will be added when needed
        return [];
    }

    public IEnumerable<Role> GetRoles()
    {
        // Empty implementation - roles will be added when needed
        return [];
    }
}
