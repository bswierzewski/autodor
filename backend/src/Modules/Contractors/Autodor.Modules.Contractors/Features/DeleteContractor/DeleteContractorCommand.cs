using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.DeleteContractor;

public record DeleteContractorCommand(
    [FromRoute] Guid Id
);
