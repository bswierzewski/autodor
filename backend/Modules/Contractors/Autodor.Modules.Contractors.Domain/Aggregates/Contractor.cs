using Autodor.Shared.Domain.Common;
using Autodor.Modules.Contractors.Domain.ValueObjects;

namespace Autodor.Modules.Contractors.Domain.Aggregates;

public class Contractor : AggregateRoot<ContractorId>
{
    public TaxId NIP { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public Email Email { get; private set; } = null!;

    private Contractor() : base(new ContractorId(Guid.Empty)) { } // EF Constructor

    public Contractor(ContractorId id, TaxId nip, string name, Address address, Email email) : base(id)
    {
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
    }

    public void UpdateDetails(string name, Address address, Email email)
    {
        Name = name;
        Address = address;
        Email = email;
        SetModifiedDate();
    }
}