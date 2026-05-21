using Microsoft.AspNetCore.Mvc;

namespace Autodor.Modules.Contractors.Features.DeleteContractor;

public class DeleteContractorCommand
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }
}