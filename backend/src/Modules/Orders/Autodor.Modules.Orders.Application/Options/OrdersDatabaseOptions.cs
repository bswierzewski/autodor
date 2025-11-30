using System.ComponentModel.DataAnnotations;
using Shared.Abstractions.Options;
using Autodor.Modules.Orders.Domain;

namespace Autodor.Modules.Orders.Application.Options;

/// <summary>
/// Database configuration options for the Orders module.
/// </summary>
public class OrdersDatabaseOptions : IOptions
{
    /// <summary>
    /// Configuration section name for binding.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}";

    /// <summary>
    /// Gets or sets the PostgreSQL connection string for Orders database.
    /// </summary>
    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
