using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.DTOs;

public class IFirmaInvoiceDto
{
    public int Zaplacono { get; set; }
    public int ZaplaconoNaDokumencie { get; set; }
    public string LiczOd { get; set; }
    public string NumerKontaBankowego { get; set; }
    public string SplitPayment { get; set; }
    public string DataWystawienia { get; set; }
    public string MiejsceWystawienia { get; set; }
    public string DataSprzedazy { get; set; }
    public string FormatDatySprzedazy { get; set; }
    public object TerminPlatnosci { get; set; }
    public string SposobZaplaty { get; set; }
    public string NazwaSeriiNumeracji { get; set; }
    public string NazwaSzablonu { get; set; }
    public string RodzajPodpisuOdbiorcy { get; set; }
    public string PodpisOdbiorcy { get; set; }
    public string PodpisWystawcy { get; set; }
    public string Uwagi { get; set; }
    public bool WidocznyNumerGios { get; set; }
    public bool WidocznyNumerBdo { get; set; }
    public int? Numer { get; set; }
    public string IdentyfikatorKontrahenta { get; set; }
    public string PrefiksUEKontrahenta { get; set; }
    public string NIPKontrahenta { get; set; }
    public IFirmaPozycje[] Pozycje { get; set; }
    public IFirmaKontrahent Kontrahent { get; set; }
}

public class IFirmaKontrahent
{
    public string Nazwa { get; set; }
    public string Nazwa2 { get; set; }
    public string Identyfikator { get; set; }
    public string PrefiksUE { get; set; }
    public string NIP { get; set; }
    public string Ulica { get; set; }
    public string KodPocztowy { get; set; }
    public string Kraj { get; set; }
    public string KodKraju { get; set; }
    public string Miejscowosc { get; set; }
    public string Email { get; set; }
    public string Telefon { get; set; }
    public bool OsobaFizyczna { get; set; }
    public bool JestOdbiorca { get; set; }
    public bool JestDostawca { get; set; }
    public bool PodmiotPowiazany { get; set; }
}

public class IFirmaPozycje
{
    public decimal StawkaVat { get; set; }
    public int Ilosc { get; set; }
    public float CenaJednostkowa { get; set; }
    public string NazwaPelna { get; set; }
    public string Jednostka { get; set; }
    public string PKWiU { get; set; }
    public string GTU { get; set; }
    public string TypStawkiVat { get; set; }
    public int Rabat { get; set; }
}

public class IFirmaResponseDto
{
    [JsonPropertyName("response")]
    public IFirmaResponse Response { get; set; }
}

public class IFirmaResponse
{
    [JsonPropertyName("Kod")]
    public int Kod { get; set; }

    [JsonPropertyName("Informacja")]
    public string Informacja { get; set; }

    [JsonPropertyName("Identyfikator")]
    public string Identyfikator { get; set; }
}