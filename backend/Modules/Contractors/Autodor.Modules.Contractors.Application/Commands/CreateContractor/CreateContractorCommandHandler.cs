using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

/// <summary>
/// Handles the creation of new contractors in the automotive parts distribution system.
/// This command handler implements the business logic for establishing new contractor relationships,
/// including validation, domain object creation, and persistence. It ensures that new contractors
/// are properly integrated into the system with all required business information.
/// </summary>
public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand, Guid>
{
    private readonly IContractorsWriteDbContext _writeDbContext;

    /// <summary>
    /// Initializes a new instance of the CreateContractorCommandHandler.
    /// The write database context is injected to enable transactional operations for contractor creation.
    /// </summary>
    /// <param name="writeDbContext">The database context for write operations, enabling contractor persistence</param>
    public CreateContractorCommandHandler(IContractorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Processes the contractor creation command by building the domain aggregate and persisting it to the database.
    /// This method orchestrates the entire contractor creation process, from generating unique identifiers
    /// to constructing value objects and ensuring transactional persistence of the new business relationship.
    /// </summary>
    /// <param name="request">The create contractor command containing all necessary business information</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <returns>The unique identifier of the newly created contractor</returns>
    public async Task<Guid> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
    {
        // Generate a new unique identifier for the contractor
        // This ensures global uniqueness across the distributed system
        var contractorId = new ContractorId(Guid.NewGuid());
        
        // Create the contractor domain aggregate with all required business information
        // The constructor enforces domain invariants and ensures data consistency
        var contractor = new Contractor(
            contractorId,
            new TaxId(request.NIP),         // Tax ID for legal compliance
            request.Name,                   // Business name for identification
            new Address(request.Street, request.City, request.ZipCode), // Physical location
            new Email(request.Email)        // Communication contact
        );

        // Add the new contractor to the database context for tracking
        // This prepares the entity for persistence without immediately saving
        await _writeDbContext.Contractors.AddAsync(contractor, cancellationToken);
        
        // Commit the transaction to persist the contractor to the database
        // This ensures atomicity and consistency of the contractor creation operation
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        // Return the contractor's unique identifier for client reference
        // This allows the client to reference the newly created contractor
        return contractorId.Value;
    }
}