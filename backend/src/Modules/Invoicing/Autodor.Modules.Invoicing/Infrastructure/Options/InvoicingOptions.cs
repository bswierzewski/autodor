using Autodor.Modules.Invoicing.Domain.Enums;
using BuildingBlocks.Kernel.Abstractions;

namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class InvoicingOptions : IOptions
{
    public static string SectionName => $"Modules:{InvoicingModule.Name}";

    public InvoiceProvider Provider { get; set; } = InvoiceProvider.InFakt;
}
