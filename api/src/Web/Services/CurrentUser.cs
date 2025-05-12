using Application.Common.Interfaces;
using System.Security.Claims;

namespace Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    /// <summary>
    /// Auth0 Id
    /// </summary>
    public string Id => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
