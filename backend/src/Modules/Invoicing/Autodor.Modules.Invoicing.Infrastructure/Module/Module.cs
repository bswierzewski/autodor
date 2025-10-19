using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Autodor.Modules.Invoicing.Infrastructure.Module;

public class Module : IModule
{
    public string ModuleName => "Invoicing";
    public string DisplayName => "Zarządzanie Fakturowaniem";
    public string? Description => "Moduł zarządzania fakturami i integracją z systemem Infakt";

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
