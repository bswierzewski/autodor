namespace Application.About.Queries.GetApplicationInfo;

public record GetApplicationInfoQuery : IRequest<ApplicationInfoDto>;

public record ApplicationInfoDto
{
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public BuildInformation BuildInfo { get; init; } = new();
}

public record BuildInformation
{
    public string GitSha { get; init; } = "N/A";
    public string BuildTimestamp { get; init; } = "N/A";
}
