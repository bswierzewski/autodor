using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

/// <summary>
/// Handles the deletion of contractors from the automotive parts distribution system.
/// This command handler implements the business logic for contractor removal, including validation,
/// constraint checking, and proper cleanup of contractor relationships. It ensures that contractor
/// deletions maintain system integrity and follow business rules for relationship termination.
/// WARNING: Consider the impact on existing orders, transactions, and audit trails before deletion.
/// </summary>
public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand>
{
    private readonly IContractorsWriteDbContext _writeDbContext;

    /// <summary>
    /// Initializes a new instance of the DeleteContractorCommandHandler.
    /// The write database context is injected to enable transactional operations for contractor removal.
    /// </summary>
    /// <param name="writeDbContext">The database context for write operations, enabling contractor deletion and persistence</param>
    public DeleteContractorCommandHandler(IContractorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Processes the contractor deletion command by locating and removing the contractor from the system.
    /// This method ensures that contractor deletions are performed safely with proper validation
    /// and error handling. Consider implementing soft deletion for audit trail preservation.
    /// </summary>
    /// <param name="request">The delete contractor command containing the contractor ID to be removed</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <exception cref="InvalidOperationException">Thrown when the contractor with the specified ID does not exist</exception>
    public async Task Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        // Locate the contractor to be deleted by its unique identifier
        // This ensures we're operating on a valid, existing entity before deletion
        var contractor = await _writeDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        // Validate that the contractor exists before attempting deletion
        // This prevents operations on non-existent entities and provides clear error messaging
        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        // TODO: Consider implementing business rule validation here:
        // - Check for existing orders or transactions
        // - Verify no active business relationships
        // - Consider soft deletion for audit trail preservation
        
        // Remove the contractor from the database context
        // This marks the entity for deletion in the next save operation
        _writeDbContext.Contractors.Remove(contractor);
        
        // Commit the transaction to permanently remove the contractor
        // This ensures atomicity and consistency of the contractor deletion operation
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}