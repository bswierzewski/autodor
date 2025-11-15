namespace Autodor.Modules.Orders.Application.Options;

/// <summary>
/// Configuration options for connecting to the Polcar sales external service.
/// </summary>
public class PolcarSalesOptions
{
    /// <summary>
    /// The configuration section name for Polcar sales options.
    /// </summary>
    public const string SectionName = "ExternalServices:Polcar:Sales";

    /// <summary>
    /// Gets or sets the login username for Polcar API authentication.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for Polcar API authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distributor code for identifying the organization in Polcar system.
    /// </summary>
    public string DistributorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch identifier for the specific branch in Polcar system.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Gets or sets the language identifier for API responses (defaults to 1).
    /// </summary>
    public int LanguageId { get; set; } = 1;
}
