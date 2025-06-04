using Application.Common.Interfaces;

namespace Application.About.Queries.GetApplicationInfo;

public class GetApplicationInfoQueryHandler : IRequestHandler<GetApplicationInfoQuery, ApplicationInfoDto>
{
    private readonly IBuildInfoService _buildInfoService;

    public GetApplicationInfoQueryHandler(IBuildInfoService buildInfoService)
    {
        _buildInfoService = buildInfoService;
    }

    public Task<ApplicationInfoDto> Handle(GetApplicationInfoQuery request, CancellationToken cancellationToken)
    {
        var buildInfo = _buildInfoService.GetBuildInformation();

        var dto = new ApplicationInfoDto
        {
            Name = "Autodor",
            Version = "0.1.0",
            BuildInfo = buildInfo
        };

        return Task.FromResult(dto);
    }
}
