using System.Net;
using System.Net.Http.Json;
using Autodor.Tests.E2E.Core;
using Autodor.Tests.E2E.Core.Extensions;
using Autodor.Tests.E2E.Core.Factories;
using BuildingBlocks.Modules.Users.Application.Queries.GetAllPermissions;
using BuildingBlocks.Modules.Users.Application.Queries.GetAllRoles;
using BuildingBlocks.Modules.Users.Application.Queries.GetCurrentUser;

namespace Autodor.Tests.E2E.Modules.Users;

/// <summary>
/// End-to-end tests for Users module functionality including permissions, roles, and user management.
/// Tests the integration of BuildingBlocks.Users with the application modules.
/// </summary>
public class UsersModuleTests(TestWebApplicationFactory factory) : TestBase(factory)
{
    protected override Task OnInitializeAsync()
    {
        // Configure HttpClient with JWT token for authenticated requests
        Client.WithBearerToken(TestJwtTokens.Default);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetPermissionsEndpoint_ShouldReturnOkWithPermissions()
    {
        // Act
        var response = await Client.GetAsync("/api/users/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var permissions = await response.Content.ReadFromJsonAsync<List<PermissionDto>>();

        // For now we don't have any permissions
        permissions.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetRolesEndpoint_ShouldReturnOkWithRoles()
    {
        // Act
        var response = await Client.GetAsync("/api/users/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();

        // For now we don't have any roles
        roles.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetCurrentUserEndpoint_ShouldReturnOkWithUserDetails()
    {
        // Act
        var response = await Client.GetAsync("/api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var currentUser = await response.Content.ReadFromJsonAsync<CurrentUserDto>();
        currentUser.Should().NotBeNull();
        currentUser!.Email.Should().Be("swierzewski.bartosz@gmail.com", "because this is the email in the test JWT token");
    }

    [Fact]
    public async Task GetPermissionsEndpoint_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        Client.WithoutAuthorization();

        // Act
        var response = await Client.GetAsync("/api/users/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, "because endpoint requires authentication");
    }

    [Fact]
    public async Task GetRolesEndpoint_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        Client.WithoutAuthorization();

        // Act
        var response = await Client.GetAsync("/api/users/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, "because endpoint requires authentication");
    }

    [Fact]
    public async Task GetCurrentUserEndpoint_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        Client.WithoutAuthorization();

        // Act
        var response = await Client.GetAsync("/api/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, "because endpoint requires authentication");
    }
}
