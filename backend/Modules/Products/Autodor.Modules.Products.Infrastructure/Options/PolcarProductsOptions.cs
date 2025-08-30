namespace Autodor.Modules.Products.Infrastructure.Options;

public class PolcarProductsOptions
{
    public const string SectionName = "ExternalServices:Polcar:Products";

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int LanguageId { get; set; } = 1;
    public int FormatId { get; set; } = 1;
}