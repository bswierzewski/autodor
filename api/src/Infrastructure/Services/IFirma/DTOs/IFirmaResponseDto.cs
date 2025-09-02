using System.Text.Json.Serialization;

namespace Infrastructure.Services.IFirma.DTOs;

public class IFirmaResponseDto
{
    [JsonPropertyName("response")]
    public IFirmaResponse Response { get; set; }
}

public class IFirmaResponse
{
    public int Kod { get; set; }
    public string Informacja { get; set; }
}