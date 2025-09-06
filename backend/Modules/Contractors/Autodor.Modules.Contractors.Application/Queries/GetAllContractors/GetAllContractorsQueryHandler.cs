using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, IEnumerable<GetAllContractorsDto>>
{
    private readonly IContractorsReadDbContext _readDbContext;

    public GetAllContractorsQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<IEnumerable<GetAllContractorsDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await _readDbContext.Contractors.ToListAsync(cancellationToken);

        return contractors.Select(contractor => new GetAllContractorsDto(
            contractor.Id.Value,
            contractor.Name,
            contractor.NIP.Value,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        ));
    }
}