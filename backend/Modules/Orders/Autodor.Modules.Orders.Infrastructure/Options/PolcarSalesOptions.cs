namespace Autodor.Modules.Orders.Infrastructure.Options;

public class PolcarSalesOptions
{
    public const string SectionName = "ExternalServices:Polcar:Sales";

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DistributorCode { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int LanguageId { get; set; } = 1;
}