using Autodor.Modules.Contractors.Domain;
using BuildingBlocks.Abstractions.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Autodor.Modules.Contractors.Application.Options;

public class ContractorsDatabaseOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
