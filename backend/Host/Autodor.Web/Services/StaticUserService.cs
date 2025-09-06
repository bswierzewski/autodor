using BuildingBlocks.Application.Abstractions;

namespace Autodor.Web.Services;

public class StaticUserService : IUser
{
    public string Id => "static-user-id";

    public string? Email => "";

    public bool IsAuthenticated => true;

    public IEnumerable<string> Claims => [];

    public IEnumerable<string> Roles => [];

    public bool HasClaim(string claimType, string? claimValue = null)
    {
        return true;
    }

    public bool IsInRole(string role)
    {
        return true;
    }
}