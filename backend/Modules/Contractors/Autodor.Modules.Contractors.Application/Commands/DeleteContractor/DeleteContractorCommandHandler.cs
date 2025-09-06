using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand>
{
    private readonly IContractorsWriteDbContext _writeDbContext;

    public DeleteContractorCommandHandler(IContractorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _writeDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        _writeDbContext.Contractors.Remove(contractor);
        await _writeDbContext.SaveChangesAsync(cancellationToken);
    }
}