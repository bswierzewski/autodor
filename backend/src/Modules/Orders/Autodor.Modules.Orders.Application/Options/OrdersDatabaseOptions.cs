using System.ComponentModel.DataAnnotations;
using BuildingBlocks.Abstractions.Abstractions;
using Autodor.Modules.Orders.Domain;

namespace Autodor.Modules.Orders.Application.Options;
public class OrdersDatabaseOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
