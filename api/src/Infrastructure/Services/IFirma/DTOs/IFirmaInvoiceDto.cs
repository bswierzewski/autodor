namespace Infrastructure.Services.IFirma.DTOs;

public class IFirmaInvoiceDto
{
    public string DataWystawienia { get; set; }
    public string DataSprzedazy { get; set; }
    public int? Numer { get; set; }
    public IFirmaPozycje[] Pozycje { get; set; }
    public IFirmaKontrahent Kontrahent { get; set; }
    public string MiejsceWystawienia { get; set; }
    public string TerminPlatnosci { get; set; }
    public decimal Zaplacono { get; set; }
    public decimal ZaplaconoNaDokumencie { get; set; }
    public string LiczOd { get; set; }
    public string FormatDatySprzedazy { get; set; }
    public string SposobZaplaty { get; set; }
    public string RodzajPodpisuOdbiorcy { get; set; }
    public string NazwaSeriiNumeracji { get; set; }
}

public class IFirmaKontrahent
{
    public string Nazwa { get; set; }
    public string NIP { get; set; }
    public string Ulica { get; set; }
    public string KodPocztowy { get; set; }
    public string Miejscowosc { get; set; }
    public string Email { get; set; }
}

public class IFirmaPozycje
{
    public decimal StawkaVat { get; set; }
    public int Ilosc { get; set; }
    public float CenaJednostkowa { get; set; }
    public string NazwaPelna { get; set; }
    public string Jednostka { get; set; }
    public string TypStawkiVat { get; set; }
}