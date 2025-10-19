using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Orders.Infrastructure.Module;

public class OrdersModule : IModule
{
    public string ModuleName => "Orders";
    public string DisplayName => "Zarządzanie Zamówieniami";
    public string? Description => "Moduł zarządzania zamówieniami i dokumentami magazynowymi";

    public IEnumerable<Permission> GetPermissions()
    {
        return new[]
        {
            Permission.Create("orders.view", "Podgląd Zamówień", ModuleName, "Podgląd listy zamówień"),
            Permission.Create("orders.create", "Tworzenie Zamówień", ModuleName, "Tworzenie nowych zamówień"),
            Permission.Create("orders.edit", "Edycja Zamówień", ModuleName, "Edycja istniejących zamówień"),
            Permission.Create("orders.exclude", "Wykluczanie Zamówień", ModuleName, "Wykluczanie zamówień z przetwarzania"),
            Permission.Create("orders.warehouse_documents", "Dokumenty Magazynowe", ModuleName, "Generowanie dokumentów magazynowych"),
        };
    }

    public IEnumerable<Role> GetRoles()
    {
        var viewPermission = Permission.Create("orders.view", "Podgląd Zamówień", ModuleName);
        var createPermission = Permission.Create("orders.create", "Tworzenie Zamówień", ModuleName);
        var editPermission = Permission.Create("orders.edit", "Edycja Zamówień", ModuleName);
        var excludePermission = Permission.Create("orders.exclude", "Wykluczanie Zamówień", ModuleName);
        var warehouseDocsPermission = Permission.Create("orders.warehouse_documents", "Dokumenty Magazynowe", ModuleName);

        var manager = Role.Create("OrdersManager", "Menedżer Zamówień", ModuleName);
        manager.AddPermission(viewPermission);
        manager.AddPermission(createPermission);
        manager.AddPermission(editPermission);
        manager.AddPermission(excludePermission);
        manager.AddPermission(warehouseDocsPermission);

        var viewer = Role.Create("OrdersViewer", "Przeglądający Zamówienia", ModuleName);
        viewer.AddPermission(viewPermission);

        return new[] { manager, viewer };
    }
}
