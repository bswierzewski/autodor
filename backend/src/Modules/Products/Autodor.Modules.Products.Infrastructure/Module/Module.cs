using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Products.Infrastructure.Module;

public class Module : IModule
{
    public string ModuleName => "Products";
    public string DisplayName => "Zarządzanie Produktami";
    public string? Description => "Moduł zarządzania produktami i synchronizacji z zewnętrznymi systemami";

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
