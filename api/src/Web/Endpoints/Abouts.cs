using Application.About.Queries.GetApplicationInfo;
using MediatR;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Abouts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetApplicationInformation);
    }

    private static async Task<ApplicationInfoDto> GetApplicationInformation(ISender sender, CancellationToken ct)
        => await sender.Send(new GetApplicationInfoQuery(), ct);
}
