using Shared.Abstractions.Options;
using Autodor.Modules.Invoicing.Domain;

namespace Autodor.Modules.Invoicing.Application.Options;

/// <summary>
/// Configuration options for InFakt invoicing service integration.
/// </summary>
public class InFaktOptions : IOptions
{
    /// <summary>
    /// The configuration section name for InFakt options.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}:InFakt";

    public string BaseUrl { get; set; } = "https://api.sandbox-infakt.pl/api/v3";
    public string ApiKey { get; set; } = string.Empty;
}
