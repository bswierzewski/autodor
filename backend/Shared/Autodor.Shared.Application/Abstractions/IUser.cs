namespace Autodor.Shared.Application.Abstractions;

public interface IUser
{
    string? Id { get; }
    string? Email { get; }
    string? Name { get; }
    bool IsAuthenticated { get; }

    Task<bool> HasPermissionAsync(string permission);
    Task<bool> IsInRoleAsync(string role);
}
