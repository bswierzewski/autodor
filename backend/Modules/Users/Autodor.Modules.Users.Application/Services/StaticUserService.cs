using Autodor.Shared.Application.Abstractions;

namespace Autodor.Modules.Users.Application.Services;

public class StaticUserService : IUser
{
    public string? Id => "automat";
    
    public string? Email => "automat@system.local";
    
    public string? Name => "System Automat";
    
    public bool IsAuthenticated => true;

    public Task<bool> HasPermissionAsync(string permission)
    {
        return Task.FromResult(true);
    }

    public Task<bool> IsInRoleAsync(string role)
    {
        return Task.FromResult(true);
    }
}