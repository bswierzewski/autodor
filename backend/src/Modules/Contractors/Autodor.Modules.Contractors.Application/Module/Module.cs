using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Contractors.Application.Module;

public class Module : IModule
{
    public string ModuleName => "Contractors";
    public string DisplayName => "Zarządzanie Kontrahentami";
    public string? Description => "Moduł zarządzania kontrahentami i ich danymi";

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
