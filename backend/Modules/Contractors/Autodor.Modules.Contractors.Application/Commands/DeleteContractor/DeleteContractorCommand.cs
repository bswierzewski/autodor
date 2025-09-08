using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

/// <summary>
/// Command for removing a contractor from the automotive parts distribution system.
/// This command represents the business intent to terminate a contractor relationship and remove
/// the contractor entity from the system. Use with caution as this operation may affect existing
/// business relationships, orders, and transaction history. Consider business rules and constraints
/// before allowing contractor deletion to maintain data integrity and audit trails.
/// </summary>
/// <param name="Id">The unique identifier of the contractor to be deleted. Must reference an existing contractor entity.</param>
public record DeleteContractorCommand(Guid Id) : IRequest;