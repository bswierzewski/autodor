using Autodor.Modules.Contractors.Domain.ValueObjects;
using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Contractors.Domain.Aggregates;

/// <summary>
/// Represents a contractor entity in the automotive parts distribution system.
/// This aggregate root encapsulates a business partner who can supply or purchase automotive parts.
/// Contractors are essential entities in the supply chain, representing suppliers, customers, or service providers
/// that the company maintains business relationships with for automotive parts distribution.
/// </summary>
public class Contractor : AggregateRoot<ContractorId>
{
    /// <summary>
    /// Gets the tax identification number (NIP) of the contractor.
    /// This is crucial for legal compliance, VAT calculations, and tax reporting requirements.
    /// </summary>
    public TaxId NIP { get; private set; } = null!;
    
    /// <summary>
    /// Gets the business name of the contractor.
    /// Used for identification, correspondence, and legal documentation in business transactions.
    /// </summary>
    public string Name { get; private set; } = null!;
    
    /// <summary>
    /// Gets the physical address of the contractor.
    /// Essential for shipping, billing, and maintaining accurate business location records.
    /// </summary>
    public Address Address { get; private set; } = null!;
    
    /// <summary>
    /// Gets the email contact information for the contractor.
    /// Primary means of electronic communication for orders, notifications, and business correspondence.
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Private parameterless constructor required by Entity Framework for materialization from database.
    /// This ensures that only the domain can create contractor instances through proper business methods.
    /// </summary>
    private Contractor() { } // EF Constructor

    /// <summary>
    /// Creates a new contractor with all required business information.
    /// This constructor ensures that a contractor cannot exist without essential business data,
    /// enforcing domain invariants and business rules from the moment of creation.
    /// </summary>
    /// <param name="id">The unique identifier for the contractor</param>
    /// <param name="nip">The tax identification number for legal compliance and business transactions</param>
    /// <param name="name">The business name used for identification and correspondence</param>
    /// <param name="address">The physical address for shipping and business location purposes</param>
    /// <param name="email">The email address for electronic communication and notifications</param>
    public Contractor(ContractorId id, TaxId nip, string name, Address address, Email email)
    {
        // Set the aggregate root ID to maintain entity identity
        Id = id;
        
        // Initialize all required business properties
        // These are essential for conducting business operations with this contractor
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
    }

    /// <summary>
    /// Updates the contractor's business details while maintaining referential integrity.
    /// This method allows for modification of contractor information while preserving the entity's identity.
    /// Used when contractor information changes due to business updates, relocations, or contact changes.
    /// </summary>
    /// <param name="name">The updated business name</param>
    /// <param name="nip">The updated tax identification number</param>
    /// <param name="address">The updated physical address</param>
    /// <param name="email">The updated email contact information</param>
    public void UpdateDetails(string name, TaxId nip, Address address, Email email)
    {
        // Update all contractor details atomically to maintain consistency
        // This ensures that partial updates cannot leave the contractor in an invalid state
        NIP = nip;
        Name = name;
        Address = address;
        Email = email;
        
        // Note: In a more complex domain, this might raise domain events
        // to notify other bounded contexts of contractor information changes
    }
}