using System.ComponentModel.DataAnnotations;
using Autodor.Modules.Invoicing.Domain.Enums;

namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class InvoicingOptions
{
    public const string SectionName = "Modules:Invoicing";

    public InvoiceProvider Provider { get; set; } = InvoiceProvider.InFakt;

    public SchedulingOptions Scheduling { get; set; } = new();
}

public class SchedulingOptions
{
    /// <summary>
    /// When true, a background job automatically creates bulk invoices on the 14th
    /// and last day of each month.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Local time at which the job runs on trigger days (14th and last day of month).
    /// </summary>
    public TimeOnly RunTime { get; set; } = new TimeOnly(21, 0);

    /// <summary>
    /// E-mail addresses that receive the post-run summary. Leave empty to skip sending.
    /// </summary>
    public List<string> SummaryRecipients { get; set; } = [];
}
