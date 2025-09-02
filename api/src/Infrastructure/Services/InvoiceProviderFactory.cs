using Application.Common.Interfaces;
using Application.Common.Options;
using Infrastructure.Services.InFakt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public interface IInvoiceProviderFactory
{
    IInvoiceService CreateInvoiceProvider();
}

public class InvoiceProviderFactory : IInvoiceProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly InvoiceProviderOptions _providerOptions;

    public InvoiceProviderFactory(IServiceProvider serviceProvider, IOptions<InvoiceProviderOptions> providerOptions)
    {
        _serviceProvider = serviceProvider;
        _providerOptions = providerOptions.Value;
    }

    public IInvoiceService CreateInvoiceProvider()
    {
        return _providerOptions.Provider switch
        {
            InvoiceProviderType.IFirma => _serviceProvider.GetRequiredService<FirmaService>(),
            InvoiceProviderType.InFakt => _serviceProvider.GetRequiredService<InFaktService>(),
            _ => throw new ArgumentException($"Unsupported invoice provider: {_providerOptions.Provider}")
        };
    }
}