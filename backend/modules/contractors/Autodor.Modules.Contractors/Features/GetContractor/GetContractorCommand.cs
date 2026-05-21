using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.GetContractor;

public class GetContractorCommand
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }
}