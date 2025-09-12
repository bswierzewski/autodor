using Autodor.Modules.Contractors.Domain.ValueObjects;
using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Contractors.Domain.Aggregates;

/// <summary>
/// Represents a contractor entity with tax information, contact details, and address.
/// </summary>
public class Contractor : AggregateRoot<ContractorId>
{
    public TaxId NIP { get; private set; } = null!;
    
    public string Name { get; private set; } = null!;
    
    public Address Address { get; private set; } = null!;
    
    public Email Email { get; private set; } = null!;

    private Contractor() { }

    /// <summary>
    /// Creates a new contractor with the specified details.
    /// </summary>
    /// <param name="id">Unique identifier for the contractor.</param>
    /// <param name="nip">Tax identification number.</param>
    /// <param name="name">Contractor name.</param>
    /// <param name="address">Contractor address.</param>
    /// <param name="email">Contact email address.</param>
    public Contractor(ContractorId id, TaxId nip, string name, Address address, Email email)
    {
        Id = id;
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
    }

    /// <summary>
    /// Updates the contractor's details with new information.
    /// </summary>
    /// <param name="name">New contractor name.</param>
    /// <param name="nip">New tax identification number.</param>
    /// <param name="address">New contractor address.</param>
    /// <param name="email">New contact email address.</param>
    public void UpdateDetails(string name, TaxId nip, Address address, Email email)
    {
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
    }
}