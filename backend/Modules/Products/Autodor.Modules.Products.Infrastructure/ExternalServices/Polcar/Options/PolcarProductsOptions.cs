namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Options;

/// <summary>
/// Configuration options for Polcar external service integration.
/// Contains authentication credentials and API parameters required for SOAP communication.
/// </summary>
public class PolcarProductsOptions
{
    /// <summary>
    /// Configuration section name for binding from appsettings.json.
    /// </summary>
    public const string SectionName = "ExternalServices:Polcar:Products";

    /// <summary>
    /// Gets or sets the login username for Polcar API authentication.
    /// This credential is provided by the Polcar service provider.
    /// </summary>
    public string Login { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the password for Polcar API authentication.
    /// Should be stored securely and not logged in plain text.
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the language identifier for product data localization.
    /// Default value of 1 typically represents the primary language (often English or local language).
    /// </summary>
    public int LanguageId { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the format identifier for response data structure.
    /// Default value of 1 represents the standard format expected by the application.
    /// </summary>
    public int FormatId { get; set; } = 1;
}