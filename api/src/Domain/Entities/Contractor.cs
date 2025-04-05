namespace Domain.Entities;
public class Contractor : BaseAuditableEntity
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string NIP { get; set; }
    public string ZipCode { get; set; }
    public string Email { get; set; }
}
