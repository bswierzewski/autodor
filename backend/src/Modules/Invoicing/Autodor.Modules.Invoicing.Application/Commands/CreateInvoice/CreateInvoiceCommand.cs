using MediatR;

namespace Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;

/// <summary>
/// Command for creating a single invoice based on specified orders and contractor.
/// This command collects order data from specified dates and creates an invoice
/// for external invoice system integration.
/// </summary>
/// <param name="InvoiceNumber">Optional invoice number, if null will be auto-generated</param>
/// <param name="SaleDate">Date when the sale transaction occurred</param>
/// <param name="IssueDate">Date when the invoice should be issued</param>
/// <param name="Dates">Collection of dates to filter orders from</param>
/// <param name="OrderIds">Collection of specific order identifiers to include</param>
/// <param name="ContractorId">Identifier of the contractor from the Contractors module</param>
public record CreateInvoiceCommand(
    int? InvoiceNumber,
    DateTime SaleDate,
    DateTime IssueDate,
    IEnumerable<DateTime> Dates,
    IEnumerable<string> OrderIds,
    Guid ContractorId
) : IRequest<string>;