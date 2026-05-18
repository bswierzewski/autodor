using Autodor.Modules.Invoicing.Domain.Enums;

namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class InvoicingOptions
{
    public const string SectionName = "Modules:Invoicing";

    public InvoiceProvider Provider { get; set; } = InvoiceProvider.InFakt;
}
