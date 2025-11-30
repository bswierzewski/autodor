using Shared.Abstractions.Options;
using Autodor.Modules.Products.Domain;

namespace Autodor.Modules.Products.Application.Options;

/// <summary>
/// Configuration options for connecting to the Polcar products external service.
/// </summary>
public class PolcarProductsOptions : IOptions
{
    /// <summary>
    /// The configuration section name for Polcar products settings.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}:Polcar:Products";

    /// <summary>
    /// Gets or sets the login username for Polcar authentication.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for Polcar authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the language identifier for product data localization.
    /// </summary>
    public int LanguageId { get; set; } = 1;

    /// <summary>
    /// Gets or sets the format identifier for the response data structure.
    /// </summary>
    public int FormatId { get; set; } = 1;
}
