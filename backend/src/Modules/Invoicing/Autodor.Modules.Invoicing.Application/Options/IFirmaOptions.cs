using Autodor.Modules.Invoicing.Domain;
using Shared.Abstractions.Options;

namespace Autodor.Modules.Invoicing.Application.Options;

/// <summary>
/// Configuration options for iFirma accounting service integration.
/// </summary>
public class IFirmaOptions : IOptions
{
    /// <summary>
    /// The configuration section name for iFirma options.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}:IFirma";

    public string BaseUrl { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public IFirmaKeys Keys { get; set; } = new();
}

/// <summary>
/// API Keys for different iFirma endpoints.
/// </summary>
public class IFirmaKeys
{
    public string? Faktura { get; set; }
    public string? Abonent { get; set; }
    public string? Rachunek { get; set; }
    public string? Wydatek { get; set; }
}
