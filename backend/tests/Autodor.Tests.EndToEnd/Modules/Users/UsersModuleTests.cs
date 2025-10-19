using System.Net;
using System.Net.Http.Json;
using Autodor.Tests.E2E.Core;
using Autodor.Tests.E2E.Core.Extensions;
using Autodor.Tests.E2E.Core.Factories;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Modules.Users.Application.Queries.GetAllPermissions;
using BuildingBlocks.Modules.Users.Application.Queries.GetAllRoles;
using BuildingBlocks.Modules.Users.Application.Queries.GetCurrentUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task GetAllPermissions_ShouldReturnAllModulePermissions()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IMediator>();

        // Act
        var query = new GetAllPermissionsQuery();
        var permissions = await mediator.Send(query);

        // Assert
        permissions.Should().NotBeNull();
        permissions.Should().NotBeEmpty("because all modules should register their permissions");

        // Verify that each module's permissions are present
        permissions.Should().Contain(p => p.ModuleName == "Contractors", "because Contractors module should register permissions");
        permissions.Should().Contain(p => p.ModuleName == "Orders", "because Orders module should register permissions");
        permissions.Should().Contain(p => p.ModuleName == "Products", "because Products module should register permissions");
        permissions.Should().Contain(p => p.ModuleName == "Invoicing", "because Invoicing module should register permissions");
    }

    [Fact]
    public async Task GetAllRoles_ShouldReturnAllModuleRoles()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IMediator>();

        // Act
        var query = new GetAllRolesQuery();
        var roles = await mediator.Send(query);

        // Assert
        roles.Should().NotBeNull();
        roles.Should().NotBeEmpty("because all modules should register their roles");

        // Verify specific roles from our modules
        roles.Should().Contain(r => r.Name == "ContractorsManager", "because Contractors module defines this role");
        roles.Should().Contain(r => r.Name == "OrdersManager", "because Orders module defines this role");
        roles.Should().Contain(r => r.Name == "ProductsManager", "because Products module defines this role");
        roles.Should().Contain(r => r.Name == "InvoicingManager", "because Invoicing module defines this role");
    }

    [Fact]
    public async Task ModulePermissions_ShouldBeCorrectlyGrouped()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IMediator>();

        // Act
        var query = new GetAllPermissionsQuery();
        var permissions = await mediator.Send(query);

        // Assert - Group by module
        var groupedPermissions = permissions.GroupBy(p => p.ModuleName).ToList();

        groupedPermissions.Count.Should().BeGreaterThanOrEqualTo(4, "At least 4 modules should have permissions");

        // Verify each module has multiple permissions
        var contractorsPermissions = permissions.Where(p => p.ModuleName == "Contractors").ToList();
        contractorsPermissions.Should().HaveCount(4, "Contractors should have view, create, edit, delete permissions");

        var ordersPermissions = permissions.Where(p => p.ModuleName == "Orders").ToList();
        ordersPermissions.Should().HaveCount(5, "Orders should have view, create, edit, exclude, warehouse_documents permissions");

        var productsPermissions = permissions.Where(p => p.ModuleName == "Products").ToList();
        productsPermissions.Should().HaveCount(2, "Products should have view and synchronize permissions");

        var invoicingPermissions = permissions.Where(p => p.ModuleName == "Invoicing").ToList();
        invoicingPermissions.Should().HaveCount(3, "Invoicing should have view, create, and bulk_create permissions");
    }

    [Fact]
    public async Task AllModules_ShouldHaveAtLeastOneRole()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IMediator>();

        // Act
        var query = new GetAllRolesQuery();
        var roles = await mediator.Send(query);

        // Assert
        roles.Count().Should().BeGreaterThanOrEqualTo(8, "Each of 4 modules should have at least 2 roles (Manager and Viewer)");

        // Verify all modules have their manager and viewer roles
        roles.Should().Contain(r => r.Name == "ContractorsManager");
        roles.Should().Contain(r => r.Name == "ContractorsViewer");
        roles.Should().Contain(r => r.Name == "OrdersManager");
        roles.Should().Contain(r => r.Name == "OrdersViewer");
        roles.Should().Contain(r => r.Name == "ProductsManager");
        roles.Should().Contain(r => r.Name == "ProductsViewer");
        roles.Should().Contain(r => r.Name == "InvoicingManager");
        roles.Should().Contain(r => r.Name == "InvoicingViewer");
    }

    #region HTTP Endpoint Tests

    [Fact]
    public async Task GetPermissionsEndpoint_ShouldReturnOkWithPermissions()
    {
        // Act
        var response = await Client.GetAsync("/api/users/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var permissions = await response.Content.ReadFromJsonAsync<List<PermissionDto>>();
        permissions.Should().NotBeNull();
        permissions.Should().NotBeEmpty("because all modules should register their permissions");

        // Verify that each module's permissions are present
        permissions.Should().Contain(p => p.ModuleName == "Contractors", "because Contractors module should register permissions");
        permissions.Should().Contain(p => p.ModuleName == "Orders", "because Orders module should register permissions");
        permissions.Should().Contain(p => p.ModuleName == "Products", "because Products module should register permissions");
        permissions.Should().Contain(p => p.ModuleName == "Invoicing", "because Invoicing module should register permissions");
    }

    [Fact]
    public async Task GetRolesEndpoint_ShouldReturnOkWithRoles()
    {
        // Act
        var response = await Client.GetAsync("/api/users/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
        roles.Should().NotBeNull();
        roles.Should().NotBeEmpty("because all modules should register their roles");

        // Verify specific roles from our modules
        roles.Should().Contain(r => r.Name == "ContractorsManager", "because Contractors module defines this role");
        roles.Should().Contain(r => r.Name == "OrdersManager", "because Orders module defines this role");
        roles.Should().Contain(r => r.Name == "ProductsManager", "because Products module defines this role");
        roles.Should().Contain(r => r.Name == "InvoicingManager", "because Invoicing module defines this role");
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

    [Fact]
    public async Task GetPermissionsEndpoint_ShouldReturnCorrectPermissionCounts()
    {
        // Act
        var response = await Client.GetAsync("/api/users/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var permissions = await response.Content.ReadFromJsonAsync<List<PermissionDto>>();
        permissions.Should().NotBeNull();

        // Verify each module has the expected number of permissions
        var contractorsPermissions = permissions!.Where(p => p.ModuleName == "Contractors").ToList();
        contractorsPermissions.Should().HaveCount(4, "Contractors should have view, create, edit, delete permissions");

        var ordersPermissions = permissions.Where(p => p.ModuleName == "Orders").ToList();
        ordersPermissions.Should().HaveCount(5, "Orders should have view, create, edit, exclude, warehouse_documents permissions");

        var productsPermissions = permissions.Where(p => p.ModuleName == "Products").ToList();
        productsPermissions.Should().HaveCount(2, "Products should have view and synchronize permissions");

        var invoicingPermissions = permissions.Where(p => p.ModuleName == "Invoicing").ToList();
        invoicingPermissions.Should().HaveCount(3, "Invoicing should have view, create, and bulk_create permissions");
    }

    [Fact]
    public async Task GetRolesEndpoint_ShouldReturnRolesWithPermissions()
    {
        // Act
        var response = await Client.GetAsync("/api/users/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
        roles.Should().NotBeNull();
        roles.Should().NotBeEmpty();

        // Verify that roles have permissions assigned
        var contractorsManager = roles!.FirstOrDefault(r => r.Name == "ContractorsManager");
        contractorsManager.Should().NotBeNull();
        contractorsManager!.PermissionNames.Should().NotBeEmpty("because ContractorsManager should have permissions");
        contractorsManager.PermissionNames.Should().Contain("contractors.view");
        contractorsManager.PermissionNames.Should().Contain("contractors.create");
        contractorsManager.PermissionNames.Should().Contain("contractors.edit");
        contractorsManager.PermissionNames.Should().Contain("contractors.delete");
    }

    #endregion
}
