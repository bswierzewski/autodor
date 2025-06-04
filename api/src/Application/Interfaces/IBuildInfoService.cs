using Application.About.Queries.GetApplicationInfo;

namespace Application.Common.Interfaces;

public interface IBuildInfoService
{
    BuildInformation GetBuildInformation();
}