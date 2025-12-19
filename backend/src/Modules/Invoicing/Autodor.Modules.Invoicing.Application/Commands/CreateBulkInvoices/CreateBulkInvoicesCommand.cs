using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;

/// <summary>
/// Command for creating multiple invoices in bulk based on a date range.
/// This command processes all orders within the specified date range
/// and creates invoices for external invoice system integration.
/// Invoices are typically grouped by contractor and date.
/// Returns a dictionary mapping contractor NIP to creation status (true if successful, false otherwise).
/// </summary>
/// <param name="DateFrom">Start date for the range of orders to process</param>
/// <param name="DateTo">End date for the range of orders to process</param>
public record CreateBulkInvoicesCommand(
    DateTime DateFrom,
    DateTime DateTo
) : IRequest<Dictionary<string, bool>>;