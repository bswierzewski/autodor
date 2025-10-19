using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Contractors.Infrastructure.Module;

public class ContractorsModule : IModule
{
    public string ModuleName => "Contractors";
    public string DisplayName => "Zarządzanie Kontrahentami";
    public string? Description => "Moduł zarządzania kontrahentami i ich danymi";

    public IEnumerable<Permission> GetPermissions()
    {
        return new[]
        {
            Permission.Create("contractors.view", "Podgląd Kontrahentów", ModuleName, "Podgląd listy kontrahentów"),
            Permission.Create("contractors.create", "Tworzenie Kontrahentów", ModuleName, "Tworzenie nowych kontrahentów"),
            Permission.Create("contractors.edit", "Edycja Kontrahentów", ModuleName, "Edycja istniejących kontrahentów"),
            Permission.Create("contractors.delete", "Usuwanie Kontrahentów", ModuleName, "Usuwanie kontrahentów"),
        };
    }

    public IEnumerable<Role> GetRoles()
    {
        var viewPermission = Permission.Create("contractors.view", "Podgląd Kontrahentów", ModuleName);
        var createPermission = Permission.Create("contractors.create", "Tworzenie Kontrahentów", ModuleName);
        var editPermission = Permission.Create("contractors.edit", "Edycja Kontrahentów", ModuleName);
        var deletePermission = Permission.Create("contractors.delete", "Usuwanie Kontrahentów", ModuleName);

        var manager = Role.Create("ContractorsManager", "Menedżer Kontrahentów", ModuleName);
        manager.AddPermission(viewPermission);
        manager.AddPermission(createPermission);
        manager.AddPermission(editPermission);
        manager.AddPermission(deletePermission);

        var viewer = Role.Create("ContractorsViewer", "Przeglądający Kontrahentów", ModuleName);
        viewer.AddPermission(viewPermission);

        return new[] { manager, viewer };
    }
}
