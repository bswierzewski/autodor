namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;

public class IFirmaOptions
{
    public const string SectionName = "Modules:Invoicing:IFirma";

    // @env: Modules__Invoicing__IFirma__BaseUrl=https://www.ifirma.pl/iapi
    public string BaseUrl { get; set; } = "https://www.ifirma.pl/iapi";

    // @env: Modules__Invoicing__IFirma__User=
    public string User { get; set; } = string.Empty;

    public IFirmaApiKeys ApiKeys { get; set; } = new();
}

public class IFirmaApiKeys
{
    // @env: Modules__Invoicing__IFirma__ApiKeys__Faktura=
    public string? Faktura { get; set; }

    // @env: Modules__Invoicing__IFirma__ApiKeys__Abonent=
    public string? Abonent { get; set; }

    // @env: Modules__Invoicing__IFirma__ApiKeys__Rachunek=
    public string? Rachunek { get; set; }

    // @env: Modules__Invoicing__IFirma__ApiKeys__Wydatek=
    public string? Wydatek { get; set; }
}
