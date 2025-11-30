using Shared.Abstractions.Modules;

namespace Autodor.Modules.Invoicing.Application;

/// <summary>
/// Marker class for the Invoicing module Application assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// </summary>
public sealed class ApplicationAssembly : IModuleAssembly
{
}
