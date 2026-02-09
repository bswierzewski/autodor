using BuildingBlocks.Kernel.Abstractions;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;

/// <summary>
/// Configuration options for Polcar external service integration
/// </summary>
public class DistributorsSalesOptions : IOptions
{
    /// <summary>
    /// Configuration section name in appsettings
    /// </summary>
    public static string SectionName => $"Modules:{OrdersModule.Name}:DistributorsSales";

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Distributor code
    /// </summary>
    public string DistributorCode { get; set; } = string.Empty;

    /// <summary>
    /// Language ID
    /// </summary>
    public int LanguageId { get; set; } = 1;

    /// <summary>
    /// Branch Id
    /// </summary>
    public int BranchId { get; set; } = 1;
}
