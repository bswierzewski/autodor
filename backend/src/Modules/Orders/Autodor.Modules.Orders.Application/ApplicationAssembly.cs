using Shared.Abstractions.Modules;

namespace Autodor.Modules.Orders.Application;

/// <summary>
/// Marker class for the Orders module Application assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// </summary>
public sealed class ApplicationAssembly : IModuleAssembly
{
}
