namespace Autodor.Modules.Orders.Features.GenerateDeliveryNote;

/// <summary>
/// Command for GenerateDeliveryNote endpoint
/// </summary>
public class GenerateDeliveryNoteCommand
{
    public string OrderId { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}
