using System.Text.Json.Serialization;

namespace Infrastructure.Services.InFakt.DTOs;

public class InFaktErrorDto
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; }
}