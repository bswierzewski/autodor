namespace Autodor.Modules.Invoicing.Domain.ValueObjects;

/// <summary>
/// Represents a single item/line on an invoice for external invoicing systems.
/// This record contains all the necessary information for a product or service
/// that appears as a line item on an invoice sent to external providers.
/// Includes pricing, tax calculations, and Polish VAT/tax classification codes.
/// </summary>
public record InvoiceItem
{
    /// <summary>
    /// Product or service name as it appears on the invoice
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Unit of measurement - defaults to "szt"
    /// </summary>
    public string Unit { get; init; } = "szt";

    /// <summary>
    /// Quantity of the item being invoiced
    /// </summary>
    public required int Quantity { get; init; }

    /// <summary>
    /// Price per single unit before discount and VAT
    /// </summary>
    public required decimal UnitPrice { get; init; }

    /// <summary>
    /// VAT rate as decimal - defaults to 0.23 (23%)
    /// </summary>
    public decimal VatRate { get; init; } = 0.23m;

    /// <summary>
    /// Type of VAT classification - defaults to "PRC"
    /// </summary>
    public string VatType { get; init; } = "PRC";

    /// <summary>
    /// Discount percentage - defaults to 0
    /// </summary>
    public decimal Discount { get; init; } = 0;

    /// <summary>
    /// Polish Classification of Products and Services code - defaults to empty
    /// </summary>
    public string PKWiU { get; init; } = "";

    /// <summary>
    /// Polish GTU (Grupowanie Transakcji Ujednoliconych) code - defaults to empty
    /// </summary>
    public string GTU { get; init; } = "";

    /// <summary>
    /// Gets the net amount after applying discount but before VAT.
    /// Calculated as: UnitPrice * Quantity * (1 - Discount/100)
    /// </summary>
    public decimal NetAmount => UnitPrice * Quantity * (1 - Discount / 100);

    /// <summary>
    /// Gets the VAT amount for this item.
    /// Calculated as: NetAmount * VatRate
    /// </summary>
    public decimal VatAmount => NetAmount * VatRate;

    /// <summary>
    /// Gets the total net amount (same as NetAmount for consistency).
    /// </summary>
    public decimal TotalNet => NetAmount;

    /// <summary>
    /// Gets the total gross amount including VAT.
    /// Calculated as: NetAmount + VatAmount
    /// </summary>
    public decimal TotalGross => NetAmount + VatAmount;
};