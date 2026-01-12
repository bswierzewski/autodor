using Autodor.Modules.Invoicing.Domain;
using Autodor.Shared.Core.Interfaces;

namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class DatabaseOptions : IDatabaseOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    public string ConnectionString { get; set; } = null!;
}
