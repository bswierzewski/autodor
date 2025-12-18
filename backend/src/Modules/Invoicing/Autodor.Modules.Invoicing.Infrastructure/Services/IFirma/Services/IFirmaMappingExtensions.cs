using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Enums;
using DomainInvoice = Autodor.Modules.Invoicing.Domain.ValueObjects.Invoice;
using DomainContractor = Autodor.Modules.Invoicing.Domain.ValueObjects.Contractor;
using DomainInvoiceItem = Autodor.Modules.Invoicing.Domain.ValueObjects.InvoiceItem;
using IFirmaInvoice = Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests.Invoice;
using IFirmaContractor = Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests.Contractor;
using IFirmaInvoiceItem = Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Models.Requests.InvoiceItem;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Services;

public static class IFirmaMappingExtensions
{
    public static IFirmaInvoice ToIFirmaInvoice(this DomainInvoice invoice)
    {
        return new IFirmaInvoice
        {
            Number = invoice.Number,
            IssueDate = DateOnly.FromDateTime(invoice.IssueDate),
            SalesDate = DateOnly.FromDateTime(invoice.SaleDate),
            IssuePlace = invoice.PlaceOfIssue,
            PaymentDeadline = DateOnly.FromDateTime(invoice.PaymentDue),
            PaymentMethod = MapPaymentMethod(invoice.PaymentMethod),
            Notes = invoice.Notes,
            Paid = 0,
            PaidOnDocument = 0,
            CalculationType = CalculationTypeEnum.NET,
            SalesDateFormat = SalesDateFormatEnum.DZN,
            RecipientSignatureType = RecipientSignatureTypeEnum.BPO,
            VisibleGiosNumber = false,
            ContractorVatNumber = invoice.Contractor.NIP,
            Contractor = invoice.Contractor.ToIFirmaContractor(),
            Items = invoice.Items.Select(item => item.ToIFirmaInvoiceItem()).ToList()
        };
    }

    public static IFirmaContractor ToIFirmaContractor(this DomainContractor contractor)
    {
        return new IFirmaContractor
        {
            Name = contractor.Name,
            VatNumber = contractor.NIP,
            Street = contractor.Street,
            PostalCode = contractor.ZipCode,
            City = contractor.City,
            Country = "Polska",
            CountryCode = "PL",
            Email = contractor.Email,
            IsRecipient = true
        };
    }

    public static IFirmaInvoiceItem ToIFirmaInvoiceItem(this DomainInvoiceItem item)
    {
        return new IFirmaInvoiceItem
        {
            Name = item.Name,
            Unit = item.Unit,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            VatRate = item.VatRate,
            VatRateType = MapVatRateType(item.VatType),
            Discount = item.Discount,
            PKWiUCode = string.IsNullOrEmpty(item.PKWiU) ? null : item.PKWiU,
            GTU = MapGtu(item.GTU)
        };
    }

    private static PaymentMethodEnum MapPaymentMethod(string paymentMethod)
    {
        return paymentMethod?.ToLowerInvariant() switch
        {
            "transfer" => PaymentMethodEnum.PRZ,
            "cash" => PaymentMethodEnum.GTK,
            "card" => PaymentMethodEnum.KAR,
            "cash_on_delivery" => PaymentMethodEnum.POB,
            "direct_debit" => PaymentMethodEnum.PZA,
            "check" => PaymentMethodEnum.CZK,
            "compensation" => PaymentMethodEnum.KOM,
            "barter" => PaymentMethodEnum.BAR,
            "dotpay" => PaymentMethodEnum.DOT,
            "paypal" => PaymentMethodEnum.PAL,
            "payu" => PaymentMethodEnum.ALG,
            "przelewy24" => PaymentMethodEnum.P24,
            "tpay" => PaymentMethodEnum.TPA,
            "electronic" => PaymentMethodEnum.ELE,
            _ => PaymentMethodEnum.PRZ // Default to bank transfer
        };
    }

    private static VatRateTypeEnum MapVatRateType(string vatType)
    {
        return vatType?.ToUpperInvariant() switch
        {
            "PRC" => VatRateTypeEnum.PRC,
            "ZW" => VatRateTypeEnum.ZW,
            _ => VatRateTypeEnum.PRC // Default to percentage
        };
    }

    private static GtuEnum? MapGtu(string gtu)
    {
        if (string.IsNullOrEmpty(gtu))
            return GtuEnum.BRAK;

        return gtu.ToUpperInvariant() switch
        {
            "GTU_01" => GtuEnum.GTU_01,
            "GTU_02" => GtuEnum.GTU_02,
            "GTU_03" => GtuEnum.GTU_03,
            "GTU_04" => GtuEnum.GTU_04,
            "GTU_05" => GtuEnum.GTU_05,
            "GTU_06" => GtuEnum.GTU_06,
            "GTU_07" => GtuEnum.GTU_07,
            "GTU_08" => GtuEnum.GTU_08,
            "GTU_09" => GtuEnum.GTU_09,
            "GTU_10" => GtuEnum.GTU_10,
            "GTU_11" => GtuEnum.GTU_11,
            "GTU_12" => GtuEnum.GTU_12,
            "GTU_13" => GtuEnum.GTU_13,
            _ => GtuEnum.BRAK
        };
    }
}
