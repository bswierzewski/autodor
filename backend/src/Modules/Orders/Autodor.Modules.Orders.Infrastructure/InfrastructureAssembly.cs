using Shared.Abstractions.Modules;

namespace Autodor.Modules.Orders.Infrastructure;

/// <summary>
/// Marker class for the Orders module Infrastructure assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// - Module endpoints (IModuleEndpoints)
/// </summary>
public sealed class InfrastructureAssembly : IModuleAssembly
{
}
