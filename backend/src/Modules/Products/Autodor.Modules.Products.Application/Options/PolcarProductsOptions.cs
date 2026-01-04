using BuildingBlocks.Abstractions.Abstractions;
using Autodor.Modules.Products.Domain;

namespace Autodor.Modules.Products.Application.Options;

public class PolcarProductsOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}:Polcar:Products";

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int LanguageId { get; set; } = 1;
    public int FormatId { get; set; } = 1;
}
