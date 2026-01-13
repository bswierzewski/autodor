using Autodor.Modules.Orders.Domain;
using Autodor.Shared.Core.Interfaces;

namespace Autodor.Modules.Orders.Infrastructure.Database;

public class DatabaseOptions : IDatabaseOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    public string ConnectionString { get; set; } = null!;
}
