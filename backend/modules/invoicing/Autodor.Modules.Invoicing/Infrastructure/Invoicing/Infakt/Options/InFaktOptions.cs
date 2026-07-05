namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;

public class InFaktOptions
{
    public const string SectionName = "Modules:Invoicing:InFakt";

    // @env: Modules__Invoicing__InFakt__BaseUrl=https://api.sandbox-infakt.pl/api/v3
    public string BaseUrl { get; set; } = "https://api.sandbox-infakt.pl/api/v3";

    // @env: Modules__Invoicing__InFakt__ApiKey=
    public string ApiKey { get; set; } = string.Empty;
}
