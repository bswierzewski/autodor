using BuildingBlocks.Core.Exceptions;

namespace Autodor.Modules.Errors.Features.BadRequest;

public static class GetBadRequestErrorHandler
{
    public static string Handle(GetBadRequestErrorCommand command)
    {
        throw new ValidationException(new Dictionary<string, string[]>
        {
            ["request"] = ["This is a simulated bad request error."]
        });
    }
}