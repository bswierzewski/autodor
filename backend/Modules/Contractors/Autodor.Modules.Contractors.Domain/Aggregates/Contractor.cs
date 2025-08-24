using Autodor.Modules.Contractors.Domain.Common;
using Autodor.Modules.Contractors.Domain.ValueObjects;

namespace Autodor.Modules.Contractors.Domain.Aggregates;

public class Contractor : AggregateRoot<ContractorId>
{
    public TaxId NIP { get; private set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public Email Email { get; private set; }

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