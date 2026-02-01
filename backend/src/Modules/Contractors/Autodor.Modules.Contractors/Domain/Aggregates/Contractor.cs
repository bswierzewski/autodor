using Autodor.Modules.Contractors.Domain.ValueObjects;
using BuildingBlocks.Kernel.Abstractions;
using BuildingBlocks.Kernel.Primitives;

namespace Autodor.Modules.Contractors.Domain.Aggregates;

public class Contractor : AuditableEntity<ContractorId>, IAggregateRoot
{
    public TaxId NIP { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public Email Email { get; private set; } = null!;

    private Contractor() { }

    public Contractor(ContractorId id, TaxId nip, string name, Address address, Email email)
    {
        Id = id;
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
    }

    public void UpdateDetails(string name, TaxId nip, Address address, Email email)
    {
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
    }
}
