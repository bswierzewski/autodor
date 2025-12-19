using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand, Unit>
{
    private readonly IContractorsDbContext _context;

    public DeleteContractorCommandHandler(IContractorsDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _context.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            throw new KeyNotFoundException($"Contractor with ID {request.Id} not found");

        _context.Contractors.Remove(contractor);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}