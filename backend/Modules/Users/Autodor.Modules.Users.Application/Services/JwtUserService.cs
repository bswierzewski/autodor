using System.Security.Claims;
using Autodor.Shared.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Autodor.Modules.Users.Application.Services;

public class JwtUserService : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => GetClaimValue(ClaimTypes.NameIdentifier) ?? GetClaimValue("sub");
    
    public string? Email => GetClaimValue(ClaimTypes.Email) ?? GetClaimValue("email");
    
    public string? Name => GetClaimValue(ClaimTypes.Name) ?? GetClaimValue("name");
    
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Task<bool> HasPermissionAsync(string permission)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return Task.FromResult(false);

        var hasPermissionClaim = user.Claims.Any(c => 
            c.Type.Equals("permission", StringComparison.OrdinalIgnoreCase) && 
            c.Value.Equals(permission, StringComparison.OrdinalIgnoreCase));

        var isAdmin = user.IsInRole("Admin");
        
        return Task.FromResult(hasPermissionClaim || isAdmin);
    }

    public Task<bool> IsInRoleAsync(string role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return Task.FromResult(user?.IsInRole(role) ?? false);
    }

    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }
}