using System.Text.Json.Serialization;

namespace Application.Invoices.Commands.DTOs;

public class InvoiceResponseDto
{
    public string CustomerNumber { get; set; }

    [JsonPropertyName("response")]
    public ResponseDto Response { get; set; }
    public override string ToString()
    {
        return $"{CustomerNumber} | {Response?.Kod} - {Response?.Informacja}";
    }
}

public class ResponseDto
{
    public int Kod { get; set; }
    public string Informacja { get; set; }
}