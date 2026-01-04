using BuildingBlocks.Abstractions.Abstractions;
using Autodor.Modules.Invoicing.Domain;

namespace Autodor.Modules.Invoicing.Application.Options;

public class InFaktOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}:InFakt";

    public string BaseUrl { get; set; } = "https://api.sandbox-infakt.pl/api/v3";
    public string ApiKey { get; set; } = string.Empty;
}
