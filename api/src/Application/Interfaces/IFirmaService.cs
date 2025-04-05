using Application.Invoices.Commands.DTOs;

namespace Application.Common.Interfaces;

public interface IFirmaService
{
    Task<HttpResponseMessage> AddInvoice(InvoiceDto invoice);
}
