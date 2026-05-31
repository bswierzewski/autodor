namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.DistributorsSales.Options;

/// <summary>
/// Configuration options for Polcar external service integration
/// </summary>
public class DistributorsSalesOptions
{
    /// <summary>
    /// Configuration section name in appsettings
    /// </summary>
    public const string SectionName = "Modules:Orders:DistributorsSales";

    /// <summary>
    /// Login
    /// </summary>
    // @env: Modules__Orders__DistributorsSales__Login=
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    // @env: Modules__Orders__DistributorsSales__Password=
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Distributor code
    /// </summary>
    // @env: Modules__Orders__DistributorsSales__DistributorCode=
    public string DistributorCode { get; set; } = string.Empty;

    /// <summary>
    /// Language ID
    /// </summary>
    // @env: Modules__Orders__DistributorsSales__LanguageId=1
    public int LanguageId { get; set; } = 1;

    /// <summary>
    /// Branch Id
    /// </summary>
    // @env: Modules__Orders__DistributorsSales__BranchId=1
    public int BranchId { get; set; } = 1;
}
