namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Polcar.Options;

/// <summary>
/// Configuration options for connecting to the Polcar Sales external service.
/// This class contains authentication credentials and service parameters required
/// for establishing secure communication with the Polcar SOAP service for order data retrieval.
/// </summary>
public class PolcarSalesOptions
{
    /// <summary>
    /// The configuration section name for Polcar Sales service settings.
    /// Used by the .NET configuration system to bind settings from appsettings.json.
    /// </summary>
    public const string SectionName = "ExternalServices:Polcar:Sales";

    /// <summary>
    /// Gets or sets the login username for authenticating with the Polcar service.
    /// This credential is required for all SOAP service calls to the Polcar system.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for authenticating with the Polcar service.
    /// This credential is paired with Login to establish authenticated sessions.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the distributor code identifying this client to the Polcar system.
    /// This code determines which distributor's data and pricing will be returned.
    /// </summary>
    public string DistributorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch identifier for filtering orders by location.
    /// Used to retrieve orders specific to a particular branch or location.
    /// </summary>
    public int BranchId { get; set; }

    /// <summary>
    /// Gets or sets the language identifier for response localization.
    /// Defaults to 1 (typically Polish) but can be configured for different languages.
    /// </summary>
    public int LanguageId { get; set; } = 1;
}