using Autodor.Modules.Invoicing.Domain;
using Autodor.Modules.Invoicing.Domain.Enums;
using BuildingBlocks.Abstractions.Abstractions;

namespace Autodor.Modules.Invoicing.Application.Options;

public class InvoicingOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    public InvoiceProvider Provider { get; set; } = InvoiceProvider.InFakt;
}
