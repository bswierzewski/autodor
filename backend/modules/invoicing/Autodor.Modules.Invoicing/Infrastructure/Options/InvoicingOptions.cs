using Autodor.Modules.Invoicing.Domain.Enums;

namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class InvoicingOptions
{
    public const string SectionName = "Modules:Invoicing";

    // @env: Modules__Invoicing__Provider=InFakt
    public InvoiceProvider Provider { get; set; } = InvoiceProvider.InFakt;

    public SchedulingOptions Scheduling { get; set; } = new();
}

public class SchedulingOptions
{
    /// <summary>
    /// When true, a background job automatically creates bulk invoices on the 14th
    /// and last day of each month.
    /// </summary>
    // @env: Modules__Invoicing__Scheduling__Enabled=false
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Local time at which the job runs on trigger days (14th and last day of month).
    /// </summary>
    // @env: Modules__Invoicing__Scheduling__RunTime=21:00:00
    public TimeOnly RunTime { get; set; } = new TimeOnly(21, 0);

    /// <summary>
    /// E-mail addresses that receive the post-run summary. Leave empty to skip sending.
    /// </summary>
    // @env: Modules__Invoicing__Scheduling__SummaryRecipients__0=
    public List<string> SummaryRecipients { get; set; } = [];
}
