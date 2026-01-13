using Autodor.Shared.Core.Interfaces;

namespace Autodor.Api;

/// <summary>
/// Dummy implementation of IUserContext for development/testing.
/// W prawdziwej aplikacji powinna być implementacja odczytująca z HttpContext.User (ClaimsPrincipal).
/// </summary>
public class DummyUserContext : IUserContext
{
    // Statyczny Guid dla testów - w prawdziwej aplikacji to byłby UserId z ClaimsPrincipal
    public Guid Id => Guid.Parse("00000000-0000-0000-0000-000000000001");

    public IEnumerable<string> Roles => new[] { "User", "Admin" };

    public bool IsInRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
