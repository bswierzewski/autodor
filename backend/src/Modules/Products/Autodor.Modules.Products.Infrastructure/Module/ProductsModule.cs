using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Products.Infrastructure.Module;

public class ProductsModule : IModule
{
    public string ModuleName => "Products";
    public string DisplayName => "Zarządzanie Produktami";
    public string? Description => "Moduł zarządzania produktami i synchronizacji z zewnętrznymi systemami";

    public IEnumerable<Permission> GetPermissions()
    {
        return new[]
        {
            Permission.Create("products.view", "Podgląd Produktów", ModuleName, "Podgląd listy produktów"),
            Permission.Create("products.synchronize", "Synchronizacja Produktów", ModuleName, "Synchronizacja produktów z zewnętrznymi systemami"),
        };
    }

    public IEnumerable<Role> GetRoles()
    {
        var viewPermission = Permission.Create("products.view", "Podgląd Produktów", ModuleName);
        var synchronizePermission = Permission.Create("products.synchronize", "Synchronizacja Produktów", ModuleName);

        var manager = Role.Create("ProductsManager", "Menedżer Produktów", ModuleName);
        manager.AddPermission(viewPermission);
        manager.AddPermission(synchronizePermission);

        var viewer = Role.Create("ProductsViewer", "Przeglądający Produkty", ModuleName);
        viewer.AddPermission(viewPermission);

        return new[] { manager, viewer };
    }
}
