using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Invoicing.Infrastructure.Module;

public class InvoicingModule : IModule
{
    public string ModuleName => "Invoicing";
    public string DisplayName => "Zarządzanie Fakturowaniem";
    public string? Description => "Moduł zarządzania fakturami i integracją z systemem Infakt";

    public IEnumerable<Permission> GetPermissions()
    {
        return new[]
        {
            Permission.Create("invoicing.view", "Podgląd Faktur", ModuleName, "Podgląd listy faktur"),
            Permission.Create("invoicing.create", "Tworzenie Faktur", ModuleName, "Tworzenie pojedynczych faktur"),
            Permission.Create("invoicing.bulk_create", "Masowe Tworzenie Faktur", ModuleName, "Tworzenie wielu faktur jednocześnie"),
        };
    }

    public IEnumerable<Role> GetRoles()
    {
        var viewPermission = Permission.Create("invoicing.view", "Podgląd Faktur", ModuleName);
        var createPermission = Permission.Create("invoicing.create", "Tworzenie Faktur", ModuleName);
        var bulkCreatePermission = Permission.Create("invoicing.bulk_create", "Masowe Tworzenie Faktur", ModuleName);

        var manager = Role.Create("InvoicingManager", "Menedżer Fakturowania", ModuleName);
        manager.AddPermission(viewPermission);
        manager.AddPermission(createPermission);
        manager.AddPermission(bulkCreatePermission);

        var viewer = Role.Create("InvoicingViewer", "Przeglądający Faktury", ModuleName);
        viewer.AddPermission(viewPermission);

        return new[] { manager, viewer };
    }
}
