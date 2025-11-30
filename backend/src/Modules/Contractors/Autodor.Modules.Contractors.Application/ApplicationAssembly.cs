using Shared.Abstractions.Modules;

namespace Autodor.Modules.Contractors.Application;

/// <summary>
/// Marker class for the Contractors module Application assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// </summary>
public sealed class ApplicationAssembly : IModuleAssembly
{
}
