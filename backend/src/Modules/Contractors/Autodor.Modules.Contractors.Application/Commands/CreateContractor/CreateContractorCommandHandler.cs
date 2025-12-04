using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

/// <summary>
/// Handles the creation of new contractors by processing CreateContractorCommand requests.
/// </summary>
public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand, Result<Guid>>
{
    private readonly IContractorsWriteDbContext _writeDbContext;

    public CreateContractorCommandHandler(IContractorsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Creates a new contractor entity and persists it to the database.
    /// </summary>
    /// <param name="request">Command containing contractor details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created contractor.</returns>
    public async Task<Result<Guid>> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractorId = new ContractorId(Guid.NewGuid());

        var contractor = new Contractor(
            contractorId,
            new TaxId(request.NIP),
            request.Name,
            new Address(request.Street, request.City, request.ZipCode),
            new Email(request.Email)
        );

        await _writeDbContext.Contractors.AddAsync(contractor, cancellationToken);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(contractorId.Value);
    }
}