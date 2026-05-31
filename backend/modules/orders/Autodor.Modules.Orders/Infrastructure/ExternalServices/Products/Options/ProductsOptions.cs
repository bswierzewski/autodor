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
    // @env: Modules__Orders__Products__Login=
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    // @env: Modules__Orders__Products__Password=
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Language ID
    /// </summary>
    // @env: Modules__Orders__Products__LanguageId=1
    public int LanguageId { get; set; } = 1;

    /// <summary>
    /// Format ID
    /// </summary>
    // @env: Modules__Orders__Products__FormatId=1
    public int FormatId { get; set; } = 1;
}
