using System.ComponentModel.DataAnnotations;
using Shared.Abstractions.Options;
using Autodor.Modules.Invoicing.Domain;

namespace Autodor.Modules.Invoicing.Application.Options;

/// <summary>
/// Database configuration options for the Invoicing module.
/// </summary>
public class InvoicingDatabaseOptions : IOptions
{
    /// <summary>
    /// Configuration section name for binding.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}";

    /// <summary>
    /// Gets or sets the PostgreSQL connection string for Invoicing database.
    /// </summary>
    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
