using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Infrastructure.Middleware;

namespace Autodor.Modules.Errors.Features.BadRequest;

public static class GetBadRequestErrorHandler
{
    [Authorize]
    public static string Handle(GetBadRequestErrorCommand command)
    {
        throw new ValidationException(new Dictionary<string, string[]>
        {
            ["request"] = ["This is a simulated bad request error."]
        });
    }
}