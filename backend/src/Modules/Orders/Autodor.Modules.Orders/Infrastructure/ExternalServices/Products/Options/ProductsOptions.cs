namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;

/// <summary>
/// Configuration options for Polcar external service integration
/// </summary>
public class ProductsOptions
{
    /// <summary>
    /// Configuration section name in appsettings
    /// </summary>
    public const string SectionName = "Modules:Orders:Products";

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Language ID
    /// </summary>
    public int LanguageId { get; set; } = 1;

    /// <summary>
    /// Format ID
    /// </summary>
    public int FormatId { get; set; } = 1;
}
