using Shared.Abstractions.Options;
using Autodor.Modules.Invoicing.Domain;

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

    public string User { get; set; } = string.Empty;
    public string Faktura { get; set; } = string.Empty;
    public string Abonent { get; set; } = string.Empty;
    public string Rachunek { get; set; } = string.Empty;
    public string Wydatek { get; set; } = string.Empty;
}
