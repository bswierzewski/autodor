using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public class UpdateContractorRequest
{
    [FromRoute]
    public Guid Id { get; set; }

    [FromBody]
    public UpdateContractorCommand? Command { get; set; }
}
