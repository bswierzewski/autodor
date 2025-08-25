using Autodor.Modules.Invoicing.Domain.Common;
using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Domain.Entities;

public class InvoiceItem : Entity<InvoiceItemId>
{
    public string PartNumber { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    private InvoiceItem() : base(new InvoiceItemId(Guid.Empty)) { } // EF Constructor

    public InvoiceItem(
        InvoiceItemId id,
        string partNumber,
        string productName,
        int quantity,
        decimal unitPrice) : base(id)
    {
        PartNumber = partNumber ?? throw new ArgumentNullException(nameof(partNumber));
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = quantity * unitPrice;
    }

    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;
        TotalPrice = Quantity * UnitPrice;
        SetModifiedDate();
    }

    public void UpdatePrice(decimal newUnitPrice)
    {
        UnitPrice = newUnitPrice;
        TotalPrice = Quantity * UnitPrice;
        SetModifiedDate();
    }
}