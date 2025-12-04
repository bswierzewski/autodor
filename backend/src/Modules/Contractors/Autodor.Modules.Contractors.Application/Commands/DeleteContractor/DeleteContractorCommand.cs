using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

/// <summary>
/// Command to delete an existing contractor.
/// </summary>
/// <param name="Id">Contractor unique identifier to delete.</param>
public record DeleteContractorCommand(Guid Id) : IRequest<Result<Unit>>;