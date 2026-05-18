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
