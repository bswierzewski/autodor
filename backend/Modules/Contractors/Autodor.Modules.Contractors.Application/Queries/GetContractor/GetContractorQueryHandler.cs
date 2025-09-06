using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public class GetContractorQueryHandler : IRequestHandler<GetContractorQuery, GetContractorDto>
{
    private readonly IContractorsReadDbContext _readDbContext;

    public GetContractorQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetContractorDto> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _readDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        return new GetContractorDto(
            contractor.Id.Value,
            contractor.Name,
            contractor.NIP.Value,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        );
    }
}