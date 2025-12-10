using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Domain.ValueObjects;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Services
{
    public class InFaktInvoiceService : IInvoiceService
    {
        public Task<string> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
