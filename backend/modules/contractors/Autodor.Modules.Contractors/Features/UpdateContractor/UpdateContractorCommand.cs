namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public class UpdateContractorCommand
{
    public Guid Id { get; set; }

    public string NIP { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
