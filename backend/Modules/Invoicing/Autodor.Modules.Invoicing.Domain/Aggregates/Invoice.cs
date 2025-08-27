using Autodor.Modules.Invoicing.Domain.Entities;
using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Modules.Invoicing.Domain.ValueObjects;
using SharedKernel.Domain.Abstractions;

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

    private Invoice() { } // EF Constructor

    public Invoice(
        string invoiceNumber,
        DateTime invoiceDate,
        Guid contractorId)
    {
        InvoiceNumber = invoiceNumber ?? throw new ArgumentNullException(nameof(invoiceNumber));
        InvoiceDate = invoiceDate;
        ContractorId = contractorId;
        Status = InvoiceStatus.Draft;
    }

    public void AddItem(InvoiceItem item)
    {
        _items.Add(item ?? throw new ArgumentNullException(nameof(item)));
    }

    public void RemoveItem(InvoiceItemId itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is not null)
        {
            _items.Remove(item);
        }
    }
}