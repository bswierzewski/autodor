using Autodor.Modules.Invoicing.Domain.ValueObjects;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.DTOs;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma;

public static class MappingExtensions
{
    public static IFirmaInvoiceDto ToInvoiceDto(this Invoice invoice)
    {
        return new IFirmaInvoiceDto
        {
            Numer = invoice.Number,
            DataWystawienia = invoice.IssueDate.ToString("yyyy-MM-dd"),
            DataSprzedazy = invoice.SaleDate.ToString("yyyy-MM-dd"),
            MiejsceWystawienia = invoice.PlaceOfIssue,
            TerminPlatnosci = invoice.PaymentDue,
            SposobZaplaty = invoice.PaymentMethod,
            Uwagi = invoice.Notes,
            Kontrahent = new IFirmaKontrahent
            {
                Nazwa = invoice.Contractor.Name,
                NIP = invoice.Contractor.NIP,
                Ulica = invoice.Contractor.Street,
                KodPocztowy = invoice.Contractor.ZipCode,
                Miejscowosc = invoice.Contractor.City,
                Kraj = "PL", // Default to Poland
                Email = invoice.Contractor.Email,
                Telefon = "" // Not available in current Contractor model
            },
            Pozycje = invoice.Items.Select(item => new IFirmaPozycje
            {
                NazwaPelna = item.Name,
                Jednostka = item.Unit,
                Ilosc = item.Quantity,
                CenaJednostkowa = (float)item.UnitPrice,
                StawkaVat = item.VatRate,
                TypStawkiVat = item.VatType,
                Rabat = (int)item.Discount,
                PKWiU = item.PKWiU,
                GTU = item.GTU
            }).ToArray()
        };
    }
}