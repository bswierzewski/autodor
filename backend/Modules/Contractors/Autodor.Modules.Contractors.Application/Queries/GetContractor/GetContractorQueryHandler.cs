using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Shared.Contracts.Contractors.Queries;
using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public class GetContractorQueryHandler : IRequestHandler<GetContractorQuery, GetContractorDto>
{
    private readonly IContractorRepository _repository;

    public GetContractorQueryHandler(IContractorRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetContractorDto> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _repository.GetByIdAsync(new ContractorId(request.Id));

        if (contractor == null)
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