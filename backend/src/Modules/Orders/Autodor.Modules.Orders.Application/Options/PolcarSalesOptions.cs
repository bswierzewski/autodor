using BuildingBlocks.Abstractions.Abstractions;
using Autodor.Modules.Orders.Domain;

namespace Autodor.Modules.Orders.Application.Options;

public class PolcarSalesOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}:Polcar:Sales";

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DistributorCode { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int LanguageId { get; set; } = 1;
}
