using System.Text.Json.Serialization;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Models.Responses;

/// <summary>
/// Response model for listing clients from InFakt API.
/// </summary>
public class Clients
{
    /// <summary>
    /// Array of client entities.
    /// </summary>
    [JsonPropertyName("entities")]
    public ICollection<Client> Entities { get; set; } = [];

    /// <summary>
    /// Current page number (for pagination).
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    [JsonPropertyName("per_page")]
    public int? PerPage { get; set; }

    /// <summary>
    /// Total number of items matching the query.
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    [JsonPropertyName("total_pages")]
    public int? TotalPages { get; set; }

    /// <summary>
    /// Current sorting order.
    /// </summary>
    [JsonPropertyName("sort")]
    public string? Sort { get; set; }

    /// <summary>
    /// Sorting direction (asc or desc).
    /// </summary>
    [JsonPropertyName("order")]
    public string? Order { get; set; }
}
