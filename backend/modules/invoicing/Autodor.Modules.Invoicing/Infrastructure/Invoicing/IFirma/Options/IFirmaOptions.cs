namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;

public class IFirmaOptions
{
    public const string SectionName = "Modules:Invoicing:IFirma";

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
