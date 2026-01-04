using Autodor.Modules.Invoicing.Domain;
using BuildingBlocks.Abstractions.Abstractions;

namespace Autodor.Modules.Invoicing.Application.Options;

public class IFirmaOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}:IFirma";

    public string BaseUrl { get; set; } = "https://www.ifirma.pl/iapi";
    public string User { get; set; } = string.Empty;
    public IFirmaApiKeys ApiKeys { get; set; } = new();
}

public class IFirmaApiKeys
{
    public string? Faktura { get; set; }
    public string? Abonent { get; set; }
    public string? Rachunek { get; set; }
    public string? Wydatek { get; set; }
}
