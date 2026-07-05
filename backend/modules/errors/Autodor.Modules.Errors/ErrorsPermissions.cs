using BuildingBlocks.Core.Primitives;

namespace Autodor.Modules.Errors;

public static class ErrorsPermissions
{
    public const string DebugSecureEndpoint = "errors:debug:secure";

    public static readonly Permission AccessSecureEndpoint = new(
        DebugSecureEndpoint,
        "Allows access to the secured errors demo endpoint.");
}
