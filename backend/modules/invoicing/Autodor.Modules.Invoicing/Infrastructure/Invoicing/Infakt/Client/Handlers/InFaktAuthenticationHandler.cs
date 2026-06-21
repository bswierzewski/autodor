using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Handlers
{
    public class InFaktAuthenticationHandler(IOptions<InFaktOptions> options) : DelegatingHandler
    {
        private readonly InFaktOptions _options = options.Value;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Add InFakt API key header to every request
            request.Headers.Add("X-inFakt-ApiKey", _options.ApiKey);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
