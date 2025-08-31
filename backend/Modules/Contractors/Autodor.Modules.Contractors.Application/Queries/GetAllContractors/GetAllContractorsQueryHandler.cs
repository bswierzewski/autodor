using Autodor.Modules.Contractors.Domain.Aggregates;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, IEnumerable<GetAllContractorsDto>>
{
    private readonly IRepository<Contractor> _repository;

    public GetAllContractorsQueryHandler(IRepository<Contractor> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GetAllContractorsDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await _repository.GetAllAsync();

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