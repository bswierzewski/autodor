using Shared.Abstractions.Modules;

namespace Autodor.Modules.Contractors.Infrastructure;

/// <summary>
/// Marker class for the Contractors module Infrastructure assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// - Module endpoints (IModuleEndpoints)
/// </summary>
public sealed class InfrastructureAssembly : IModuleAssembly
{
}
