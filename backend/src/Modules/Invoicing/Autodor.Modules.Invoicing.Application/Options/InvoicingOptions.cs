using Shared.Abstractions.Options;
using Autodor.Modules.Invoicing.Domain;

namespace Autodor.Modules.Invoicing.Application.Options;

/// <summary>
/// Configuration options for the Invoicing module.
/// </summary>
public class InvoicingOptions : IOptions
{
    /// <summary>
    /// The configuration section name for Invoicing options.
    /// </summary>
    public static string SectionName => $"Modules:{ModuleConstants.ModuleName}";

    /// <summary>
    /// The invoice provider to use for invoice generation.
    /// </summary>
    public InvoiceProvider Provider { get; set; } = InvoiceProvider.InFakt;
}
