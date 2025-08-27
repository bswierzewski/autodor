using SharedKernel.Application.Interfaces;

namespace Autodor.Web.Services;

public class StaticUserService : IUser
{
    public string ExternalId => "static-user-001";
    
    public int Id => 1;
    
    public bool IsAuthenticated => true;

    public Task<bool> HasPermissionAsync(string permission)
    {
        // Na początek zwracamy true dla wszystkich uprawnień
        // Później zostanie uzupełnione o prawdziwą logikę
        return Task.FromResult(true);
    }

    public Task<bool> IsInRoleAsync(string role)
    {
        // Na początek zwracamy true dla roli "Admin"
        // Później zostanie uzupełnione o prawdziwą logikę
        return Task.FromResult(role.Equals("Admin", StringComparison.OrdinalIgnoreCase));
    }
}