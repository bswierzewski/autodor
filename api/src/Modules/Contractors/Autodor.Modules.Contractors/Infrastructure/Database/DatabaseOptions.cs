using Autodor.Modules.Contractors.Domain;
using Autodor.Shared.Core.Interfaces;

namespace Autodor.Modules.Contractors.Infrastructure.Database;

public class DatabaseOptions : IDatabaseOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    public string ConnectionString { get; set; } = null!;
}
