using Application.Invoices.Commands.DTOs;

namespace Application.Invoices.Extensions
{
    public static class InvoiceExtensions
    {
        public static InvoiceDto CreateDefaultInvoiceDto(List<Pozycje> pozycje)
        {
            return new InvoiceDto()
            {
                MiejsceWystawienia = "Leszno",
                TerminPlatnosci = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd"),
                Zaplacono = 0,
                ZaplaconoNaDokumencie = 0,
                LiczOd = "BRT",
                FormatDatySprzedazy = "DZN",
                SposobZaplaty = "PRZ",
                RodzajPodpisuOdbiorcy = "OUP",
                NazwaSeriiNumeracji = "Domyślna roczna",
                Pozycje = pozycje.ToArray(),
            };
        }
    }
}
