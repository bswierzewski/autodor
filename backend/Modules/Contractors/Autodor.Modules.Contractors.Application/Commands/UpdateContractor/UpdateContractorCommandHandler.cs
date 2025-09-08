using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

/// <summary>
/// Handles the updating of existing contractor information in the automotive parts distribution system.
/// This command handler implements the business logic for modifying contractor details while maintaining
/// data integrity, entity relationships, and business rule compliance. It ensures that contractor updates
/// are properly validated and persisted within a transactional context.
/// </summary>
public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand>
{
    private readonly IContractorsWriteDbContext _writeDbContext;

    /// <summary>
    /// Initializes a new instance of the UpdateContractorCommandHandler.
    /// The write database context is injected to enable transactional operations for contractor modifications.
    /// </summary>
    /// <param name="writeDbContext">The database context for write operations, enabling contractor updates and persistence</param>
    public UpdateContractorCommandHandler(IContractorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Processes the contractor update command by locating the existing contractor and applying the new business information.
    /// This method ensures that contractor updates maintain data consistency and business rule compliance,
    /// while providing proper error handling for non-existent contractors.
    /// </summary>
    /// <param name="request">The update contractor command containing the contractor ID and updated business information</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <exception cref="InvalidOperationException">Thrown when the contractor with the specified ID does not exist</exception>
    public async Task Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        // Locate the existing contractor by its unique identifier
        // This ensures we're operating on a valid, existing entity
        var contractor = await _writeDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        // Validate that the contractor exists before attempting updates
        // This prevents operations on non-existent entities and provides clear error messaging
        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        // Apply the updated business information using the domain aggregate's method
        // This ensures that business rules and invariants are enforced during the update
        contractor.UpdateDetails(
            request.Name,                   // Updated business name
            new TaxId(request.NIP),         // Updated tax identification
            new Address(request.Street, request.City, request.ZipCode), // Updated physical location
            new Email(request.Email)        // Updated communication contact
        );

        // Mark the contractor as modified in the Entity Framework change tracker
        // This ensures the changes are included in the next save operation
        _writeDbContext.Contractors.Update(contractor);
        
        // Commit the transaction to persist the updated contractor information
        // This ensures atomicity and consistency of the contractor update operation
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}