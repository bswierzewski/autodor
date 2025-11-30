using Shared.Abstractions.Modules;

namespace Autodor.Modules.Products.Infrastructure;

/// <summary>
/// Marker class for the Products module Infrastructure assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// - Module endpoints (IModuleEndpoints)
/// </summary>
public sealed class InfrastructureAssembly : IModuleAssembly
{
}
