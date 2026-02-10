using BuildingBlocks.Kernel.Abstractions;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;

public class InFaktOptions : IOptions
{
    public static string SectionName => $"Modules:{InvoicingModule.Name}:InFakt";

    public string BaseUrl { get; set; } = "https://api.sandbox-infakt.pl/api/v3";
    public string ApiKey { get; set; } = string.Empty;
}
