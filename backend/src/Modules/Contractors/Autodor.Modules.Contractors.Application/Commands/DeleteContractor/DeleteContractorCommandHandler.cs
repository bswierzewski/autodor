using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand, Result<Unit>>
{
    private readonly IContractorsDbContext _context;

    public DeleteContractorCommandHandler(IContractorsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _context.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            return Result<Unit>.Failure("CONTRACTOR_NOT_FOUND", $"Contractor with ID {request.Id} not found");

        _context.Contractors.Remove(contractor);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}