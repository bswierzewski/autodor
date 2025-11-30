using System.ComponentModel.DataAnnotations;
using Shared.Abstractions.Options;
using Autodor.Modules.Contractors.Domain;

namespace Autodor.Modules.Contractors.Application.Options;

/// <summary>
/// Database configuration options for the Contractors module.
/// </summary>
public class ContractorsDatabaseOptions : IOptions
{
    /// <summary>
    /// Configuration section name for binding.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}";

    /// <summary>
    /// Gets or sets the PostgreSQL connection string for Contractors database.
    /// </summary>
    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
