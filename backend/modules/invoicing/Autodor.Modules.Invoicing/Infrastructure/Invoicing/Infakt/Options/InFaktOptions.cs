namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;

public class InFaktOptions
{
    public const string SectionName = "Modules:Invoicing:InFakt";

    public string BaseUrl { get; set; } = "https://api.sandbox-infakt.pl/api/v3";
    public string ApiKey { get; set; } = string.Empty;
}
