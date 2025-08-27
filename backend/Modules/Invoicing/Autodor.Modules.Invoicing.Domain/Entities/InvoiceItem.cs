using Autodor.Modules.Invoicing.Domain.ValueObjects;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Invoicing.Domain.Entities;

public class InvoiceItem : Entity<InvoiceItemId>
{
    public string PartNumber { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    private InvoiceItem() { } // EF Constructor

    public InvoiceItem(
        InvoiceItemId id,
        string partNumber,
        string productName,
        int quantity,
        decimal unitPrice)
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
    }

    public void UpdatePrice(decimal newUnitPrice)
    {
        UnitPrice = newUnitPrice;
        TotalPrice = Quantity * UnitPrice;
    }
}