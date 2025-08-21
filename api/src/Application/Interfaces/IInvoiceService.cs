using Application.Common;
using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IInvoiceService
{
    Task<Result<string>> AddInvoice(Invoice invoice);
}
