using Autodor.Shared.Domain.Common;
using Autodor.Modules.Invoicing.Domain.Entities;
using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Domain.Aggregates;

public class Invoice : AggregateRoot<InvoiceId>
{
    private readonly List<InvoiceItem> _items = new();

    public string InvoiceNumber { get; private set; } = null!;
    public DateTime InvoiceDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid ContractorId { get; private set; }
    public IReadOnlyList<InvoiceItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(i => i.TotalPrice);

    private Invoice() : base(new InvoiceId(Guid.Empty)) { } // EF Constructor

    public Invoice(
        InvoiceId id,
        string invoiceNumber,
        DateTime invoiceDate,
        Guid contractorId) : base(id)
    {
        InvoiceNumber = invoiceNumber ?? throw new ArgumentNullException(nameof(invoiceNumber));
        InvoiceDate = invoiceDate;
        ContractorId = contractorId;
        Status = InvoiceStatus.Draft;
    }

    public void AddItem(InvoiceItem item)
    {
        _items.Add(item ?? throw new ArgumentNullException(nameof(item)));
        SetModifiedDate();
    }

    public void RemoveItem(InvoiceItemId itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is not null)
        {
            _items.Remove(item);
            SetModifiedDate();
        }
    }

    public void ChangeStatus(InvoiceStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        SetModifiedDate();
    }

    public void Send()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be sent");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot send invoice without items");

        Status = InvoiceStatus.Sent;
        SetModifiedDate();
    }

    public void MarkAsPaid()
    {
        if (Status != InvoiceStatus.Sent)
            throw new InvalidOperationException("Only sent invoices can be marked as paid");

        Status = InvoiceStatus.Paid;
        SetModifiedDate();
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot cancel paid invoices");

        Status = InvoiceStatus.Cancelled;
        SetModifiedDate();
    }
}