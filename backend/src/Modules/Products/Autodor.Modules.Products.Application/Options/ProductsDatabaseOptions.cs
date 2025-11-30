using System.ComponentModel.DataAnnotations;
using Shared.Abstractions.Options;
using Autodor.Modules.Products.Domain;

namespace Autodor.Modules.Products.Application.Options;

/// <summary>
/// Database configuration options for the Products module.
/// </summary>
public class ProductsDatabaseOptions : IOptions
{
    /// <summary>
    /// Configuration section name for binding.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}";

    /// <summary>
    /// Gets or sets the PostgreSQL connection string for Products database.
    /// </summary>
    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
