using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.GetContractors;

public class GetContractorsCommand
{
    [FromQuery(Name = "nips")]
    public string[]? NIPs { get; set; }
}
