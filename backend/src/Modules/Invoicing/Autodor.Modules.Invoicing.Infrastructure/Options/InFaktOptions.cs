namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class InFaktOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://api.sandbox-infakt.pl/api/v3";
}