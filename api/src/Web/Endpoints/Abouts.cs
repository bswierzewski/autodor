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

    private static async Task<IResult> GetApplicationInformation(ISender sender, CancellationToken ct)
    {
        var query = new GetApplicationInfoQuery();
        var result = await sender.Send(query, ct);
        return TypedResults.Ok(result);
    }
}
