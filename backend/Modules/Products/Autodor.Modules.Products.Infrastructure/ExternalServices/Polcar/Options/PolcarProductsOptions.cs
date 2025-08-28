namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;

public class PolcarProductsOptions
{
    public const string SectionName = "ExternalServices:Polcar:Products";

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DistributorCode { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int LanguageId { get; set; } = 1;
}