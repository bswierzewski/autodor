namespace Application.Common.Options;

public class InvoiceProviderOptions
{
    public InvoiceProviderType Provider { get; set; } = InvoiceProviderType.IFirma;
}

public enum InvoiceProviderType
{
    IFirma = 0,
    InFakt = 1
}