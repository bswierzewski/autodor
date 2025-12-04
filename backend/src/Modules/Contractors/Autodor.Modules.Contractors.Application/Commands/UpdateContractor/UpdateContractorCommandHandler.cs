using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand, Result<Unit>>
{
    private readonly IContractorsWriteDbContext _writeDbContext;

    public UpdateContractorCommandHandler(IContractorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<Result<Unit>> Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _writeDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            return Result<Unit>.Failure("CONTRACTOR_NOT_FOUND", $"Contractor with ID {request.Id} not found");

        contractor.UpdateDetails(
            request.Name,
            new TaxId(request.NIP),
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );

        _writeDbContext.Contractors.Update(contractor);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}