namespace Autodor.Modules.Invoicing.Domain.ValueObjects;

/// <summary>
/// Represents a single item/line on an invoice for external invoicing systems.
/// This record contains all the necessary information for a product or service
/// that appears as a line item on an invoice sent to external providers.
/// Includes pricing, tax calculations, and Polish VAT/tax classification codes.
/// </summary>
/// <param name="Name">Product or service name as it appears on the invoice</param>
/// <param name="Unit">Unit of measurement (e.g., "sztuk", "kg", "m")</param>
/// <param name="Quantity">Quantity of the item being invoiced</param>
/// <param name="UnitPrice">Price per single unit before discount and VAT</param>
/// <param name="VatRate">VAT rate as decimal (e.g., 0.23 for 23%)</param>
/// <param name="VatType">Type of VAT classification for this item</param>
/// <param name="Discount">Discount percentage (e.g., 10 for 10% discount)</param>
/// <param name="PKWiU">Polish Classification of Products and Services code</param>
/// <param name="GTU">Polish GTU (Grupowanie Transakcji Ujednoliconych) code for tax purposes</param>
public record InvoiceItem(
    string Name,
    string Unit,
    int Quantity,
    decimal UnitPrice,
    decimal VatRate,
    string VatType,
    decimal Discount,
    string PKWiU,
    string GTU
)
{
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