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

    public string BaseUrl { get; set; } = "https://www.ifirma.pl/";
    public string User { get; set; } = string.Empty;
    public IFirmaApiKeys ApiKeys { get; set; } = new();
}

/// <summary>
/// API Keys for different iFirma endpoints.
/// </summary>
public class IFirmaApiKeys
{
    public string? Faktura { get; set; }
    public string? Abonent { get; set; }
    public string? Rachunek { get; set; }
    public string? Wydatek { get; set; }
}
